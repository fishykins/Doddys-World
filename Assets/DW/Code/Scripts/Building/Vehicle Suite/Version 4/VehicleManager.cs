using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unifish.DataTypes;
using Unifish.Grids;
using Unifish.Meshes;
using Unifish;

namespace DW.Building.VehicleSuite4 {
    [RequireComponent(typeof(MeshManager))]
    public class VehicleManager : MonoBehaviour
    {
        #region Variables
        //Public & Serialized
        public short gridSize = 8;
        public float gridScale = 0.3f;
        public int generatorRadius = 6;
        public int defaultCubeSize = 1;
        public float randomGeneratorVolume = 0.02f;
        public float generationThickness = 0.2f;
        public Vector3 gridCentre = Vector3.zero;
        public GenerationType generationType = GenerationType.sphere;

        public Material mainMaterial;
        public Material ghostMaterial;
        public Material selectedMaterial;
        public GameObject pointerPrefab;

        public float cameraRange = 20f;
        public float cameraDistance = 10f;
        public float raycastMultiplier = 1f;
        public EditMode editMode = EditMode.select;
        public KeyCode keyBake = KeyCode.B;
        public KeyCode keyDragSelect = KeyCode.LeftShift;
        public KeyCode keySingleAxis = KeyCode.LeftControl;
        public KeyCode keyCycleMode = KeyCode.F;
        public float scrollWheelSensitivity = 3f;
        public float singleClickTime = 0.1f;

        public string directoryName = "Vehicle Builder";
        public string fileName = "default";

        public int vertsPerMesh = 10000;
        public int shapesPerFrame = 1;
        public int maxSelectedShaped = 64;
        public int meshCompressionRate = 0;

        public string debugInfo = "Debug info";
        public Volume selection; //The selection made by the player
        public Grid3D<IShape> geometry;

        //Private
        private string folderPath;
        private int optimizationIndex = 0;
        private int optimizationCount = 0;
        private GameObject pointer;
        private BuildingLayer layer = BuildingLayer.geometry;
        private MeshManager meshManager;

        //Raycasting & Input
        private byte axisCount = 2;
        private Vector3 axisAnchor = Vector3.zero;
        private Vector3 worldHit; //Poisition in world where raycast hit
        private Vector3 selectionStartPosition; //Poisition in world where selection started
        private bool selectionStarted = false;
        private float selectionStartTime;

        #endregion;

        #region Properties
        public Grid3D<IShape> Grid { get { return geometry; } }
        public int OptimizationIndex { get { return optimizationIndex; } }
        public int OptimizationCount { get { return optimizationCount; } }
        #endregion;

        #region Unity Methods
        private void Awake()
        {
            meshManager = GetComponent<MeshManager>();
            if (!meshManager) Debug.LogError("No MeshManger found- consider adding!");
            folderPath = Path.Combine(Application.persistentDataPath, directoryName);
            Short3 point = Short3.one * gridSize;
            Volume gridVolume = new Volume(-point, point);
            geometry = new Grid3D<IShape>(gridVolume, false);
            selection = new Volume(Short3.zero);

            if (pointerPrefab) {
                pointer = Instantiate(pointerPrefab);
            } else Debug.LogWarning("No pointer prefab defined!");
            

            //GenerateDefaultShape();
        }

        private void Update()
        {
            debugInfo = ""; //Clear debug info each frame
            HandleRay();
            HandleInput();

            if (pointer) {
                //Short3 gridPos = BuildingUtil.WorldToGrid(worldHit, gridCentre, gridScale);
                pointer.transform.position = worldHit;// BuildingUtil.GridToWorld(gridPos, gridCentre, gridScale);
            }

            //deal with the demo blocks
            if (selectionStarted) {
                meshManager.BakeSelection(selection);
            } else {
                meshManager.BakeSelection(selection);
                //meshManager.ClearSelection();
            }
        }
        #endregion

        #region Generation
        private void GenerateDefaultShape()
        {
            if (generationType == GenerationType.single) {
                Short3 point = Short3.one * (defaultCubeSize - 1);
                Volume startingVolume = new Volume(-point, point);
                IShape newShape = new Cuboid();
                geometry.AddObject(newShape, startingVolume);
                Bake();
                return;
            }
        }

        private void Bake()
        {

        }
        #endregion

        #region Casting
        private void HandleRay()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 point = Vector3.zero;

            if (selectionStarted) {
                //Mouse is down
                switch (axisCount) {
                    case 1:
                        //We are locked to a single axis- find the axis with the largest distance and use that
                        point = ray.origin + ray.direction * cameraDistance;

                        Vector3 singleAxisPoint = axisAnchor + new Vector3(point.x - axisAnchor.x, 0f, 0f);

                        float dist = Mathf.Abs(point.y - axisAnchor.y);
                        if (dist > Mathf.Abs(point.x - axisAnchor.x)) {
                            singleAxisPoint = axisAnchor + new Vector3(0f, point.y - axisAnchor.y, 0f);
                        }
                        if (Mathf.Abs(axisAnchor.z - point.z) > dist) {
                            singleAxisPoint = axisAnchor + new Vector3(0f, 0f, point.z - axisAnchor.z);
                        }
                        point = singleAxisPoint;
                        break;
                    case 2:
                        //Two axis- snap to a plane
                        Vector3 intersect = Vector3.zero;

                        Math3D.LinePlaneIntersection(out point, ray.origin, ray.direction, Vector3.right, axisAnchor);

                        if (Math3D.LinePlaneIntersection(out intersect, ray.origin, ray.direction, Vector3.up, axisAnchor)) {
                            Vector3 direction = Vector3.up - intersect;
                            if (direction.sqrMagnitude < (Vector3.right - point).sqrMagnitude) {
                                point = intersect;
                            }
                        }
                        if (Math3D.LinePlaneIntersection(out intersect, ray.origin, ray.direction, Vector3.forward, axisAnchor)) {
                            Vector3 direction = Vector3.forward - intersect;
                            if (direction.sqrMagnitude < (Vector3.right - point).sqrMagnitude) {
                                point = intersect;
                            }
                        }
                        break;
                    case 3:
                        point = simpleRaycast(ray);
                        break;
                }
            } else {
                //Free selection
                point = simpleRaycast(ray);
            }

            SetSelection(point);
        }

        private Vector3 simpleRaycast(Ray ray)
        {
            RaycastHit hit;

            if (UnityEngine.Physics.Raycast(ray, out hit, cameraDistance * raycastMultiplier)) {
                //Hit a target
                return hit.point + hit.normal;
            }
            else {
                //No hit
                return ray.origin + ray.direction * cameraDistance;
            }
        }
        #endregion

        #region Building
        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0)) {
                StartSelection();
            }

            if (Input.GetMouseButtonUp(0)) {
                EndSelection();

                switch (layer) {
                    case BuildingLayer.geometry:
                        if (geometry.AddObject(new Cuboid(), selection)) {
                            Debug.Log(selection);
                            meshManager.BakeGeometry();
                        }
                        break;
                    default:
                        break;
                }
            }

            if (Input.GetMouseButtonUp(1) && Input.GetKey(KeyCode.LeftShift)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (UnityEngine.Physics.Raycast(ray, out hit, cameraDistance * raycastMultiplier)) {
                    //Hit a target
                    Vector3 gridPosition = BuildingUtil.WorldToGrid(hit.point, gridCentre, gridScale);
                    IShape shape = geometry.GetNearestObject(gridPosition);
                    if (shape != null) {
                        geometry.RemoveObject(shape);
                        meshManager.BakeGeometry();
                    }
                }
            }

            if (Input.GetKey(keyDragSelect)) {
                axisCount = 3;
            } else {
                if (Input.GetKey(keySingleAxis)) {
                    axisCount = 1;
                } else {
                    axisCount = 2;
                }
            }

            cameraDistance = Mathf.Clamp(cameraDistance + (Input.GetAxis("Mouse ScrollWheel") * scrollWheelSensitivity), 5f, cameraRange);
        }

        private void SetSelection(Vector3 worldPosition)
        {
            Vector3 gridPosition = BuildingUtil.WorldToGrid(worldPosition, gridCentre, gridScale);
            float selectionDuration = Time.time - selectionStartTime;

            if (selectionDuration > singleClickTime) {
                //Long click
                if (selectionStarted) {
                    //We are doing a drag selection
                    selection.SetUV(selectionStartPosition, gridPosition);
                } else {
                    //Just select the current position
                    selection.SetUV(gridPosition, gridPosition);
                }
            } else {
                //Short click
                selection.SetUV(selectionStartPosition, selectionStartPosition);
            }

            worldHit = worldPosition;
        }
        private void StartSelection()
        {
            if (selectionStarted) return;
            selectionStartTime = Time.time;
            selectionStartPosition = BuildingUtil.WorldToGrid(worldHit, gridCentre, gridScale);
            axisAnchor = worldHit;
            selectionStarted = true;
        }
        private void EndSelection()
        {
            selectionStarted = false;
        }
        #endregion
    }

    #region Enums
    public enum EditMode
    {
        select,
        build,
        deform
    }
    public enum GenerationType
    {
        sphere,
        cube,
        single,
    }

    public enum BuildingLayer
    {
        geometry,
    }
    #endregion
}
