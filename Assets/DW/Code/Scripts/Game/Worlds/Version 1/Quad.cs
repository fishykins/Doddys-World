using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Fishy.DataTypes;
using Fishy.Meshes;
using Fishy.Noise;


namespace DW.Worlds.V1 {
	public class Quad {
        #region Variables
        //Public & Serialized
        public string index;
        public Vector3 transformPosition;
        public Quaternion rotation;
        public World world;
        public float scale = 1f;
        public QuadPlane plane = 0;
        public Position position = 0;
        public bool disabled = false;
        public int level = 0;
        public bool hasSplit = false;
        public bool isSplitting;
        public bool initialized;
        public bool isComputingOnGPU;
        public ComputeBuffer computeBuffer;
        public Coroutine coroutine;
        public float distance; //used by comparer

        public string[] neighborIds;
        public Quad[] neighbors;
        public Quad[] children;
        public Quad parent;
        public bool4 quadConfiguration;
        public GameObject renderedQuad;

        public Mesh mesh;
        public Vector3 meshOffset;

        //Private
        private bool4 quadConfigurationOld = bool4.True;
        private Func<MeshData, MeshData> method;
        private IAsyncResult asyncResult;
        
        
        private AsyncGPUReadbackRequest gpuReadbackRequest;

        private float averageHeight = 0f;
        private Vector2 heightMinMax;

        #endregion;

        #region Properties

        #endregion;

        #region Constructors
        /// <summary>
        /// Create a new Quad with trPosition and rotation
        /// </summary>
        public Quad(Vector3 position, Quaternion rotation)
        {
            this.transformPosition = position;
            this.rotation = rotation;

            heightMinMax.x = float.MaxValue;
            heightMinMax.y = float.MinValue;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Creates a MeshData that is later applied to this Quad's mesh.
        /// We are implimneting cube UV type, until we get gud
        /// </summary>
        private MeshData GenerateWorldMeshData(MeshData meshData)
        {

            Vector3[] finalVerts = new Vector3[1089];
            Vector3 down = Vector3.zero;
            double offsetX = 0, offsetY = 0;
            int levelConstant = 0;
            float heightSum = 0f;

            //Cube UV
            levelConstant = (int)Mathf.Pow(2, 0);

            char[] chars = index.Substring(2).ToCharArray();
            double p = 0.5; //0.5 *  world.uvScale;
            for (int j = 0; j < chars.Length; j++) {
                switch (chars[j]) {
                    case '0':
                        break;
                    case '1':
                        offsetX += p;
                        break;
                    case '2':
                        offsetY += p;
                        break;
                    case '3':
                        offsetX += p;
                        offsetY += p;
                        break;
                }
                p *= 0.5;

            }

            //Compute shader
            meshOffset = meshData.vertices[0];
            down = (Vector3.zero - meshOffset).normalized;


            for (int i = 0; i < 1089; i++) 
            {
                meshData.vertices[i] -= meshOffset;
                finalVerts[i] = meshData.vertices[i];

                //Used by several calculations
                Vector3 worldVector = meshOffset + meshData.vertices[i] + transformPosition;

                //UV calculations (cube)
                double x = (i / 33) / 32.0;
                double y = (i % 33) / 32.0;

                double scale = (double)1f / levelConstant;

                x *= scale;
                y *= scale;

                x += (offsetX % 1);
                y += (offsetY % 1);

                y *= -1;

                //Terrain height
                float height = worldVector.magnitude;
                float min = world.radius + world.elevationMinMax.x;
                float max = min + world.elevationMinMax.y;
                float relativeHeight = Mathf.InverseLerp(min, max, height);
                    //((height / world.radius) - 1f) * world.heightInverted;

                heightSum += height;

                if (height > heightMinMax.y) heightMinMax.y = height;
                if (height < heightMinMax.x) heightMinMax.x = height;


                //Biome
                float biome = world.noise.Evaluate(worldVector.normalized * world.biomeScale.x);
                float biome2 = world.noise.Evaluate((worldVector.normalized + world.biomeOffset) * world.biomeScale.y);

                meshData.uv[i] = new Vector2((float)x, (float)y);

                meshData.colors[i] = new Color(relativeHeight, biome, biome2, 0.24f);
                meshData.uv2[i] = new Vector2(1f, 1f);
            }

            for (int i = 1089; i < meshData.vertices.Length; i++) {
                meshData.vertices[i] -= meshOffset;
            }

            meshData.normals = CalculateNormals(ref meshData.vertices, PlanetaryTerrain.ConstantTriArrays.trisExtendedPlane);
            meshData.vertices = finalVerts;

            averageHeight = heightSum / meshData.vertices.Length;

            //Debug.Log(index + " average height = " +averageHeight + ", minMax = " + heightMinMax);

            return meshData;
        }

        private Vector3[] CalculateNormals(ref Vector3[] vertices, int[] tris)
        {
            Vector3[] normals = new Vector3[vertices.Length];
            Vector3[] finalNormals = new Vector3[1089];

            for (int i = 0; i < tris.Length; i += 3) {
                Vector3 p1 = vertices[tris[i]];
                Vector3 p2 = vertices[tris[i + 1]];
                Vector3 p3 = vertices[tris[i + 2]];

                Vector3 l1 = p2 - p1;
                Vector3 l2 = p3 - p1;

                Vector3 normal = Vector3.Cross(l1, l2);

                normals[tris[i]] += normal;
                normals[tris[i + 1]] += normal;
                normals[tris[i + 2]] += normal;
            }

            for (int i = 0; i < 1089; i++) {
                finalNormals[i] = normals[i].normalized;
            }

            return finalNormals;
        }

        /// <summary>
        /// Applies MeshData to this Quad's mesh
        /// </summary>
        private void ApplyToMesh(MeshData meshData)
        {
            if (!mesh)
                mesh = new Mesh();
            else
                mesh.Clear();

            mesh.vertices = meshData.vertices;
            mesh.triangles = PlanetaryTerrain.Utils.GetTriangles(quadConfiguration.ToString());
            mesh.colors32 = meshData.colors;
            mesh.uv = meshData.uv;
            mesh.uv4 = meshData.uv2;

            mesh.RecalculateBounds();
            mesh.normals = meshData.normals;
            initialized = true;

            if (renderedQuad)
                renderedQuad.GetComponent<MeshFilter>().mesh = mesh;
        }

        /// <summary>
        /// Checks GPU and Async threads
        /// </summary>
        private void HandleThreads()
        {
            if (asyncResult != null && asyncResult.IsCompleted) {
                MeshData result = method.EndInvoke(asyncResult);
                ApplyToMesh(result);
                UpdateDistances();
                asyncResult = null;
                method = null;
            }

            //Checks to see if we have a GPU compute going on
            if (isComputingOnGPU && gpuReadbackRequest.done) {
                isComputingOnGPU = false;

                if (gpuReadbackRequest.hasError) {
                    computeBuffer.Dispose();
                    computeBuffer = null;
                    quadConfigurationOld = bool4.True;
                    GetNeighbors();
                }
                else {
                    var data = gpuReadbackRequest.GetData<Vector3>().ToArray();
                    MeshData meshData = new MeshData(data, world.quadMesh.normals, world.quadMesh.uv);

                    //Apply meshData to a shpere!
                    method = GenerateWorldMeshData;
                    asyncResult = method.BeginInvoke(meshData, null, null);

                    computeBuffer.Dispose();
                    computeBuffer = null;
                }
            }
        }

        /// <summary>
        /// Removes all of this quad's children and reenables rendering
        /// </summary>
        private void Combine()
        {
            //Debug.Log(index + " is combining!");

            if (hasSplit && !isSplitting) {
                hasSplit = false;
                foreach (Quad quad in children) {
                    if (quad.hasSplit)
                        quad.Combine();

                    world.RemoveQuad(quad);
                }
                children = null;
                UpdateNeighbors();
                
            }
        }

        /// <summary>
        /// Update neighbors in all neighbors and all their children
        /// </summary>
        private void UpdateNeighbors()
        {
            if (neighbors != null)
                foreach (Quad quad in neighbors) {
                    if (quad != null)
                        quad.GetNeighborsAll();
                }
        }

        /// <summary>
        /// Update neighbors in this Quad and all children
        /// </summary>
        private void GetNeighborsAll()
        {
            if (initialized) {
                if (children != null)
                    foreach (Quad quad in children)
                        quad.GetNeighborsAll();
                GetNeighbors();
                UpdateDistances();
            }
        }

        private void AddToQuadSplitQueue()
        {
            if (!world.quadSplitQueue.Contains(this) && !hasSplit)
                world.quadSplitQueue.Add(this);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Main update loop called by World
        /// </summary>
        public void Update(bool changedViewport = false)
        {
            HandleThreads();

            if (changedViewport) {
                UpdateDistances();
            }
        }

        /// <summary>
        /// Decides how/what to render
        /// </summary>
        public void UpdateDistances()
        {
            if (world.trackedObjects.Count == 0 || !initialized) return;

            //Get distances to every tracked object...

            if (world.trackedObjects.Count == 1) {
                //only one tracked object so dont bother doing loop
                distance = mesh.bounds.SqrDistance(world.trackedObjects[0].worldToMeshVector - meshOffset);
            } else {
                distance = float.MaxValue;

                if (mesh != null && initialized) {
                    for (int i = 0; i < world.trackedObjects.Count; i++) {
                        TrackedObject trackedObj = world.trackedObjects[i];
                        float meshDistance = mesh.bounds.SqrDistance(trackedObj.worldToMeshVector - meshOffset);
                        if (meshDistance < distance) distance = meshDistance;
                    }
                }
            }

            //Decide if we need to split or what
            if (level < world.detailDistancesSqr.Length) {
                //Split
                if (distance < world.detailDistancesSqr[level]) {
                    AddToQuadSplitQueue();
                }

                //Combine
                if (distance > world.detailDistancesSqr[level]) {
                    if (world.quadSplitQueue.Contains(this) && !isSplitting) 
                        world.quadSplitQueue.Remove(this);

                    Combine();
                }
            }

            //Update the gameObject used for rendering
            if (!renderedQuad && initialized && !hasSplit)
                renderedQuad = world.RenderQuad(this);

            if (renderedQuad && !renderedQuad.activeSelf && (level == 0 || parent.hasSplit))
                renderedQuad.SetActive(true);
        }

        /// <summary>
        /// Finds all neighbors to this quad and updates configuration if needs be
        /// </summary>
        public void GetNeighbors()
        {
            //Calculate what our neighnors are going to be
            if (neighborIds == null) {

                neighborIds = new string[4];
                for (int i = 0; i < 4; i++) {
                    neighborIds[i] = PlanetaryTerrain.QuadNeighbor.GetNeighbor(index, i.ToString());
                }
            }

            //Find the actual quads assosiated with our predicted ids
            neighbors = new Quad[4];
            for (int i = 0; i < neighbors.Length; i++) {
                int j = 0;
                while (neighbors[i] == null && j < 4) {
                    neighbors[i] = world.FindQuad(neighborIds[i].Substring(0, neighborIds[i].Length - j));
                    j++;
                }
            }

            //check our current config and update if needbe
            quadConfiguration = bool4.False;
            for (int i = 0; i < neighbors.Length; i++) //Creating configuration based on neighbor levels.
            {
                if (neighbors[i] != null) {
                    if (neighbors[i].level == level - 1)
                        quadConfiguration[i] = true;
                    else
                        quadConfiguration[i] = false;
                }
                else
                    quadConfiguration[i] = false;
            }

            //Loading plane mesh and starting generation on another thread or GPU.
            if (quadConfiguration != quadConfigurationOld)
            {
                quadConfigurationOld = new bool4(quadConfiguration);

                if (!initialized) {
                    MeshData meshData = new MeshData(PlanetaryTerrain.ConstantTriArrays.extendedPlane, world.quadMesh.normals, world.quadMesh.uv);

                    int kernelIndex = world.computeShader.FindKernel("ComputePositions");

                    computeBuffer = new ComputeBuffer(1225, 12);
                    computeBuffer.SetData(PlanetaryTerrain.ConstantTriArrays.extendedPlane);

                    //These are all set in stone until we can rework the computeShader generator from PT. Shouldnt really matter for the moment
                    world.computeShader.SetFloat("scale", scale);
                    world.computeShader.SetFloats("trPosition", new float[] { transformPosition.x, transformPosition.y, transformPosition.z });
                    world.computeShader.SetFloat("radius", world.radius);
                    world.computeShader.SetFloats("rotation", new float[] { rotation.x, rotation.y, rotation.z, rotation.w });
                    world.computeShader.SetFloat("noiseDiv", 1f / world.heightScale);

                    world.computeShader.SetBuffer(kernelIndex, "dataBuffer", computeBuffer);
                    world.computeShader.Dispatch(kernelIndex, 5, 1, 1);
                    gpuReadbackRequest = AsyncGPUReadback.Request(computeBuffer);
                    isComputingOnGPU = true;

                } else {
                    if (mesh.vertices.Length > 0)
                        mesh.triangles = PlanetaryTerrain.Utils.GetTriangles(quadConfiguration.ToString());
                    if (renderedQuad)
                        renderedQuad.GetComponent<MeshFilter>().mesh = mesh;
                }
            }
        }

        public IEnumerator Split()
        {
            if (hasSplit) yield break;

            isSplitting = true;
            children = new Quad[4];

            int[] order = WorldLibrary.OrderOfQuadChildren[new int2((int)plane, (int)position)]; //This is some magic shit I dont understand

            //Create children using dark magic
            switch (plane) {
                case QuadPlane.XPlane:

                    children[order[0]] = world.NewQuad(new Vector3(transformPosition.x, transformPosition.y - 1f / 2f * scale, transformPosition.z - 1f / 2f * scale), rotation);
                    children[order[1]] = world.NewQuad(new Vector3(transformPosition.x, transformPosition.y + 1f / 2f * scale, transformPosition.z - 1f / 2f * scale), rotation);
                    children[order[2]] = world.NewQuad(new Vector3(transformPosition.x, transformPosition.y + 1f / 2f * scale, transformPosition.z + 1f / 2f * scale), rotation);
                    children[order[3]] = world.NewQuad(new Vector3(transformPosition.x, transformPosition.y - 1f / 2f * scale, transformPosition.z + 1f / 2f * scale), rotation);
                    break;

                case QuadPlane.YPlane:

                    children[order[0]] = world.NewQuad(new Vector3(transformPosition.x - 1f / 2f * scale, transformPosition.y, transformPosition.z - 1f / 2f * scale), rotation);
                    children[order[1]] = world.NewQuad(new Vector3(transformPosition.x + 1f / 2f * scale, transformPosition.y, transformPosition.z - 1f / 2f * scale), rotation);
                    children[order[2]] = world.NewQuad(new Vector3(transformPosition.x + 1f / 2f * scale, transformPosition.y, transformPosition.z + 1f / 2f * scale), rotation);
                    children[order[3]] = world.NewQuad(new Vector3(transformPosition.x - 1f / 2f * scale, transformPosition.y, transformPosition.z + 1f / 2f * scale), rotation);
                    break;

                case QuadPlane.ZPlane:

                    children[order[0]] = world.NewQuad(new Vector3(transformPosition.x - 1f / 2f * scale, transformPosition.y - 1f / 2f * scale, transformPosition.z), rotation);
                    children[order[1]] = world.NewQuad(new Vector3(transformPosition.x + 1f / 2f * scale, transformPosition.y - 1f / 2f * scale, transformPosition.z), rotation);
                    children[order[2]] = world.NewQuad(new Vector3(transformPosition.x + 1f / 2f * scale, transformPosition.y + 1f / 2f * scale, transformPosition.z), rotation);
                    children[order[3]] = world.NewQuad(new Vector3(transformPosition.x - 1f / 2f * scale, transformPosition.y + 1f / 2f * scale, transformPosition.z), rotation);
                    break;
            }

            //Set children variables
            for (int i = 0; i < children.Length; i++) {
                children[i].scale = scale / 2;
                children[i].level = level + 1;
                children[i].plane = plane;
                children[i].parent = this;
                children[i].world = world;
                children[i].index = index + i;
                children[i].position = position;
                world.quadIndices.Add(children[i].index, children[i]);
            }

            //Neighbors
            for (int i = 0; i < children.Length; i++) {
                children[i].GetNeighbors();
                world.Quads.Add(children[i]);
            }

            //Wait for children to update
            for (int i = 0; i < children.Length; i++) {
                yield return new WaitUntil(() => children[i].initialized);
                children[i].Update(true);
            }

            //Render!
            for (int i = 0; i < children.Length; i++) { 
                if (children[i].renderedQuad)
                    children[i].renderedQuad.SetActive(true);
            }

            if (renderedQuad)
                world.RemoveRenderedQuad(this);

            UpdateNeighbors();

            isSplitting = false;
            hasSplit = true;
            coroutine = null;
        }
        #endregion
    }
}
