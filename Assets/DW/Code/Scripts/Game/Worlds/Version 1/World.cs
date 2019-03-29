using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fishy.DataTypes;
using Fishy.Noise;

namespace DW.Worlds.V1 {
    public class World : MonoBehaviour {
        #region Variables
        //Public & Serialized

        public ComputeShader computeShader;
        public Material material;
        public float radius = 1000f;
        public float heightScale = 0.2f;
        public int biomeSeed = 1;
        public Vector3 biomeOffset = Vector3.one;
        public Vector2 biomeScale = Vector2.one;
        public Vector2 elevationMinMax;
        public float boundsRange = 20f;
        public AnimationCurve lodDistanceCurve = AnimationCurve.Linear(1f, 1f, 0f, 0f);
        public int lodLevels = 3;
        public Vector2 lodRange = new Vector2(50f, 5000f);
        public int maxQuadSplitsPerFrame = 4;

        public float testRange = 1000f;

        public Transform[] watchObjects;

        public string inspectorLog;

        //Internal
        internal Vector3 position;
        internal float heightInverted;
        internal Hashtable quadIndices = new Hashtable();
        internal Mesh quadMesh;
        internal Noise3D noise;
        internal List<TrackedObject> trackedObjects = new List<TrackedObject>();
        internal float[] detailDistances;
        internal float[] detailDistancesSqr;
        internal List<Quad> quadSplitQueue = new List<Quad>();


        //Private
        private bool initialized = false;
        private List<Quad> quads = new List<Quad>();
        private GameObject quadPrefab;
        private float sqrBoundsRange;

        QuadSorter sorter;

        #endregion;

        #region Properties
        public bool Initialized { get { return initialized; } }
        public List<Quad> Quads { get { return quads; } }
        #endregion;

        #region Unity Methods
        void Start() {
            Initialize();

            StartCoroutine(InstantiateWorld());
        }

        /// <summary>
        /// UPDATE LOOP!
        /// </summary>
		void Update() {
            inspectorLog = "";

            DequeueSplittingQuads();

            bool changedViewport = false;

            inspectorLog += "Tracked Objects: ";
            foreach (var trackedObj in trackedObjects) {

                trackedObj.Update();

                if (trackedObj.BoundsChanged(sqrBoundsRange)) {
                    changedViewport = true;
                    break;
                }

                inspectorLog += trackedObj.name + ", ";
            }

            UpdateQuads(changedViewport);

            inspectorLog += "\n" + "Quad splitting Queue: ";
            for (int i = 0; i < quadSplitQueue.Count; i++) {
                inspectorLog += quadSplitQueue[i].index + ", ";
            }
        }
        #endregion;

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        private void Initialize()
        {
            quadMesh = ((GameObject)Resources.Load("planeMesh")).GetComponent<MeshFilter>().sharedMesh;
            quadPrefab = ((GameObject)Resources.Load("plane")); //Original renderQuad
            sorter = new QuadSorter();
            noise = new Noise3D(biomeSeed);
            sqrBoundsRange = boundsRange * boundsRange;

            position = transform.position;
            heightInverted = 1f / heightScale;

            //Setup all tracked obejcts
            for (int i = 0; i < watchObjects.Length; i++) {
                trackedObjects.Add(new TrackedObject(watchObjects[i], transform));
            }

            //Calculate all lod levels
            detailDistances = new float[lodLevels];
            detailDistancesSqr = new float[lodLevels];

            for (int i = 0; i < lodLevels; i++) {
                float lerp = lodDistanceCurve.Evaluate(i / ((float)lodLevels - 1));
                detailDistances[i] = Mathf.Lerp(lodRange.x, lodRange.y, lerp);
                detailDistancesSqr[i] = detailDistances[i] * detailDistances[i];
                Debug.Log("LOD level " + i + " reached at " + detailDistances[i] + " meters");
            }
        }

        private IEnumerator InstantiateWorld()
        {
            #region Boring Quad stuff
            quads.Add(new Quad(Vector3.up, Quaternion.Euler(0, 180, 0)));
            quads[0].plane = QuadPlane.YPlane;
            quads[0].position = Position.Front;
            quads[0].index = "01"; //Check QuadNeighbor.cs for an explaination of indices.

            quads.Add(new Quad(Vector3.down, Quaternion.Euler(180, 180, 0)));
            quads[1].plane = QuadPlane.YPlane;
            quads[1].position = Position.Back;
            quads[1].index = "21";

            quads.Add(new Quad(Vector3.forward, Quaternion.Euler(270, 270, 270)));
            quads[2].plane = QuadPlane.ZPlane;
            quads[2].position = Position.Front;
            quads[2].index = "03";

            quads.Add(new Quad(Vector3.back, Quaternion.Euler(270, 0, 0)));
            quads[3].plane = QuadPlane.ZPlane;
            quads[3].position = Position.Back;
            quads[3].index = "13";

            quads.Add(new Quad(Vector3.right, Quaternion.Euler(270, 0, 270)));
            quads[4].plane = QuadPlane.XPlane;
            quads[4].position = Position.Front;
            quads[4].index = "02";

            quads.Add(new Quad(Vector3.left, Quaternion.Euler(270, 0, 90)));
            quads[5].plane = QuadPlane.XPlane;
            quads[5].position = Position.Back;
            quads[5].index = "12";
            #endregion

            for (int i = 0; i < quads.Count; i++) {
                quads[i].world = this;
                quads[i].disabled = false;
                quadIndices.Add(quads[i].index, quads[i]);
            }

            //Now we have records of all our quads, we can fire them off
            for (int i = 0; i < quads.Count; i++) {
                quads[i].GetNeighbors();
            }

            initialized = true;
            yield return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="changedViewport"></param>
        private void UpdateQuads(bool changedViewport)
        {
            for (int i = 0; i < quads.Count; i++) {
                if (!quads[i].disabled)
                    quads[i].Update(changedViewport);
                else
                    quads.Remove(quads[i]);
            }
        }

        private void DequeueSplittingQuads()
        {
            //Quad Split Queue
            if (quadSplitQueue.Count > 0) { 
                if (quadSplitQueue.Count > maxQuadSplitsPerFrame) {
                    quadSplitQueue.Sort(maxQuadSplitsPerFrame - 1, quadSplitQueue.Count - maxQuadSplitsPerFrame, sorter);
                }

                for (int i = 0; i < maxQuadSplitsPerFrame; i++) {
                    if (quadSplitQueue.Count >= i + 1) {
                        if (quadSplitQueue[i] == null)
                            quadSplitQueue.Remove(quadSplitQueue[i]);

                        if (!quadSplitQueue[i].isSplitting && quadSplitQueue[i].coroutine == null && quadSplitQueue[i].world)
                            quadSplitQueue[i].coroutine = StartCoroutine(quadSplitQueue[i].Split());

                        if (quadSplitQueue[i].hasSplit) //Wait until quad has split, then spot is freed
                            quadSplitQueue.Remove(quadSplitQueue[i]);
                    }
                    else break; //No more to split- stop!
                }
            }
        }
        #endregion

        #region Public Methods
        public GameObject RenderQuad(Quad quad)
        {
            GameObject renderedQuad;

            renderedQuad = (GameObject)Instantiate(quadPrefab, transform.rotation * (quad.transformPosition + quad.meshOffset) + transform.position, transform.rotation);
            renderedQuad.GetComponent<MeshRenderer>().material = material;

            renderedQuad.layer = gameObject.layer;
            renderedQuad.GetComponent<MeshFilter>().mesh = quad.mesh;
            renderedQuad.name = "Quad " + quad.index;

            return renderedQuad;
        }

        public void RemoveRenderedQuad(Quad quad)
        {
            Destroy(quad.renderedQuad);
        }

        public void AddTrackedObject(Transform trackedObject)
        {
            trackedObjects.Add(new TrackedObject(trackedObject, transform));
        }

        /// <summary>
        /// Returns a new Quad, either from the pool if possible or instantiated
        /// </summary>
        public Quad NewQuad(Vector3 transformPosition, Quaternion rotation)
        {
            return new Quad(transformPosition, rotation);
        }

        /// <summary>
        /// Finds quad by index
        /// </summary>
        public Quad FindQuad(string index)
        {
            return (Quad)quadIndices[index];
        }

        public void RemoveQuad(Quad quad)
        {
            quad.disabled = true;

            if (quad.isComputingOnGPU) {
                quad.computeBuffer.Dispose();
                quad.computeBuffer = null;
            }

            if (quad.children != null)
                for (int i = 0; i < quad.children.Length; i++)
                    RemoveQuad(quad.children[i]);

            if (quad.renderedQuad)
                RemoveRenderedQuad(quad);

            Destroy(quad.mesh);
            quads.Remove(quad);
            quadSplitQueue.Remove(quad);

            if (quad.index != null)
                quadIndices.Remove(quad.index);
        }
        #endregion
    }
}
