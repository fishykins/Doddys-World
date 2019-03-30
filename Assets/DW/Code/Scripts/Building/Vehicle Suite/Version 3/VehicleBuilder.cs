using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unifish;
using Unifish.Grids;
using Unifish.DataTypes;

namespace DW.Building.VehicleSuite3 {
	public class VehicleBuilder : MonoBehaviour {
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
        public EditMode editMode = EditMode.select;
        public KeyCode keyBake = KeyCode.B;
        public KeyCode keyDragSelect = KeyCode.LeftShift;
        public KeyCode keyCycleMode = KeyCode.F;

        public string directoryName = "Vehicle Builder";
        public string fileName = "default";

        public int vertsPerMesh = 10000;
        public int shapesPerFrame = 1;
        public int maxSelectedShaped = 64;
        public int meshCompressionRate = 0;

        public string debugInfo = "Debug info";

        //Private
        private Grid3D<IShape> grid;
        private MeshBaker baker;
        private string folderPath;
        private int optimizationIndex = 0;
        private int optimizationCount = 0;

        private List<IShape> selectedShapes = new List<IShape>();

        //Input 
        private GameObject pointerBlock; //Our demo block to move around and show where blocks can be placed
        private GameObject pointer; //Our demo block to move around and show where blocks can be placed
        private Short3 gridHit; //The grid in which the raycast hit
        private Vector3 worldHit; //The grid in which the raycast hit
        private Short3 gridNormal; //The grid in which the raycast hit, plus the hit normal
        private Volume selection; //The selection made by the player
        private IShape shapeHit; // The shape which the raycast hit

        private bool snapToAxis = false;
        
        #endregion;

        #region Properties
        public Grid3D<IShape> Grid { get { return grid; } }
        public int OptimizationIndex { get { return optimizationIndex; } }
        public int OptimizationCount { get { return optimizationCount; } }
        #endregion;

        #region Unity Methods
        private void Awake()
        {
            debugInfo = "Debug info";
            folderPath = Path.Combine(Application.persistentDataPath, directoryName);
            Short3 point = Short3.one * gridSize;
            Volume gridVolume = new Volume(-point, point);
            selection = new Volume(-point, point);
            grid = new Grid3D<IShape>(gridVolume, false);
            baker = new MeshBaker(this);

            //Pointers
            pointerBlock = baker.GeneratePointerShape();
            pointer = Instantiate(pointerPrefab);
            pointer.transform.SetParent(transform);
            pointer.GetComponent<Renderer>().material = mainMaterial;
            pointer.SetActive(false);

            //GridDrawer.GenerateGridLines(grid, Vector3.zero, gridScale, selectedMaterial);

            GenerateGrid();
            Bake(1f);
            StartCoroutine(OptimizeGrid(1));
        }

        private IEnumerator OptimizeGrid(int timeBetweenLoops)
        {
            while (true) {

                IShape[] shapes = grid.GetObjects().ToArray();
                optimizationCount = shapes.Length;

                if (shapesPerFrame > 0) {

                    int i = 0;

                    for (optimizationIndex = 0; optimizationIndex < optimizationCount; optimizationIndex++, i++) {
                        shapes[optimizationIndex].CalculateNeighbors(grid, false);

                        if (i >= shapesPerFrame) {
                            i = 0;
                            yield return null;
                        }

                    }
                    Bake();
                    //loop = false; //For the moment, abort after the first run through.
                }

                yield return new WaitForSeconds(timeBetweenLoops);
            }
        }

        private void Update()
        {
            debugInfo = "";
            HandleRaycast();
            HandleInput();
        }
        #endregion;

        #region Input Management
        private void HandleRaycast()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!snapToAxis) {
                RaycastHit hit;

                if (UnityEngine.Physics.Raycast(ray, out hit, cameraRange)) {
                    gridHit = grid.GetNearestOccupiedPosition(WorldToGrid(hit.point));
                    worldHit = hit.point;
                    gridNormal = gridHit + (Short3)hit.normal;
                    if (editMode == EditMode.build) {
                        pointerBlock.SetActive(true);
                        baker.UpdatePointerMesh(pointerBlock, gridNormal);
                        //pointerBlock.transform.position = GridToWorld(gridNormal);
                    }
                }
                else {
                    DisablePointerBlock();
                }
            }
            else {
                //Calculate axis crossing
                Vector3 intersectX = Vector3.zero;
                Vector3 intersectY = Vector3.zero;
                Vector3 intersectZ = Vector3.zero;
                Vector3 intersect = Vector3.zero;

                Vector3 selectionPos = GridToWorld(gridNormal);

                if (Math3D.LinePlaneIntersection(out intersectX, ray.origin, ray.direction, Vector3.right, selectionPos)) {
                    Vector3 direction = Vector3.right - intersectX;
                    intersect = WorldToGrid(intersectX);
                }
                if (Math3D.LinePlaneIntersection(out intersectY, ray.origin, ray.direction, Vector3.up, selectionPos)) {
                    Vector3 direction = Vector3.up - intersectY;
                    if (direction.sqrMagnitude < (Vector3.right - intersectX).sqrMagnitude) {
                        intersect = WorldToGrid(intersectY);
                    }
                }
                if (Math3D.LinePlaneIntersection(out intersectZ, ray.origin, ray.direction, Vector3.forward, selectionPos)) {
                    Vector3 direction = Vector3.forward - intersectZ;
                    if (direction.sqrMagnitude < (Vector3.right - intersectY).sqrMagnitude) {
                        intersect = WorldToGrid(intersectZ);
                    }
                }

                selection.SetUV(intersect, gridNormal);
                baker.UpdatePointerMesh(pointerBlock, selection);
            }
        }

        private void DisablePointerBlock()
        {
            pointerBlock.SetActive(false);
            baker.UpdatePointerMesh(pointerBlock, Short3.zero);
        }
        private void DisableSelectionMode()
        {
            selectedShapes = new List<IShape>();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(keyBake)) Bake(0);
            if (Input.GetKeyDown(keyCycleMode)) CycleEditMode();
            if (Input.GetKeyDown(KeyCode.Alpha1)) CycleEditMode(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) CycleEditMode(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) CycleEditMode(2);

            if (Input.GetMouseButtonDown(0)) {
                switch (editMode) {
                    case EditMode.select:
                        if (selectedShapes.Count < maxSelectedShaped) {
                            IShape shape = grid.GetObject(gridHit);
                            selectedShapes.Add(shape);
                            Bake();
                        }
                        
                        break;
                    case EditMode.build:
     
                        snapToAxis = Input.GetKey(keyDragSelect);
      
                        break;
                    case EditMode.deform:
                        IShape selectedShape = grid.GetObject(gridHit);
                        Volume volume = grid.GetVolume(selectedShape);
                        Vector3 worldPosition = volume.Center * gridScale;
                        Vector3 scale = new Vector3(volume.Width, volume.Height, volume.Depth) * gridScale;
                        Short3 vertex = selectedShape.GetClosestVertex(worldHit, worldPosition, scale);
                        Debug.Log("Nearest Vertex to " + worldHit + ": " + vertex);
                        pointer.SetActive(true);
                        pointer.transform.position = GridToWorld(vertex);
                        break;
                    default:
                        break;
                }
            }
            if (Input.GetMouseButtonUp(0)) {
                switch (editMode) {
                    case EditMode.select:
                        break;
                    case EditMode.build:
                        DefautShape newShape = new DefautShape();
                        Volume newVolume = (snapToAxis) ? selection : gridNormal;
                        if (grid.AddObject(newShape, newVolume)) newShape.CalculateNeighbors(grid, true);
                        Bake();
                        break;
                    case EditMode.deform:
                        break;
                    default:
                        break;
                }
                snapToAxis = false;
            }

            if (Input.GetMouseButtonDown(1)) {
                switch (editMode) {
                    case EditMode.select:
                        IShape shape = grid.GetObject(gridHit);
                        selectedShapes.Remove(shape);
                        Bake();
                        break;
                    case EditMode.build:
                        IShape deadShape = grid.GetObject(gridHit);
                        List<Volume> edges = grid.GetAdjoiningVolumes(grid.GetVolume(deadShape));
                        grid.RemoveObject(deadShape);

                        foreach (Volume edge in edges) {
                            foreach (IShape neighbor in grid.GetObjects(edge)) {
                                neighbor.CalculateNeighbors(grid, false);
                            }
                        }

                        Bake();
                        break;
                    case EditMode.deform:
                        break;
                    default:
                        break;
                }
            }
        }

        private void CycleEditMode(int mode = -1)
        {
            editMode = (mode == -1) ? editMode + 1 : (EditMode)mode;
            if ((int)editMode > 2) {
                editMode = 0;
            }

            switch (editMode) {
                case EditMode.select:
                    DisablePointerBlock();
                    break;
                case EditMode.build:
                    break;
                case EditMode.deform:
                    DisablePointerBlock();
                    break;
                default:
                    break;
            }
        }

        private void SetMaterial(Transform transform, Material material)
        {
            if (transform) {
                Renderer renderer = transform.GetComponent<Renderer>();
                if (material && renderer) {
                    renderer.material = material;
                }
            }
        }
        #endregion

        #region Private Methods
        private void Bake(float speed = 0.5f)
        {
            baker.BakeToMesh(selectedShapes, speed);
        }

        private Vector3 GridToWorld(Short3 gridPosition)
        {
            return new Vector3(gridPosition.x * gridScale, gridPosition.y * gridScale, gridPosition.z * gridScale);
        }

        private Vector3 WorldToGrid(Vector3 worldPosition)
        {
            return new Vector3(worldPosition.x / gridScale, worldPosition.y / gridScale, worldPosition.z / gridScale);
        }


        private void GenerateGrid()
        {
            if (generationType == GenerationType.single) {
                Short3 point = Short3.one * (defaultCubeSize - 1);
                Volume startingVolume = new Volume(-point, point);
                grid.AddObject(new DefautShape(), startingVolume);
                return;
            }

            //This is terrible- so ineficient
            for (short x = grid.Volume.u.x; x < grid.Volume.v.x; x++) {
                for (short y = grid.Volume.u.y; y < grid.Volume.v.y; y++) {
                    for (short z = grid.Volume.u.z; z < grid.Volume.v.z; z++) {

                        if (Random.Range(0f, 1f) <= randomGeneratorVolume) {

                            Short3 gridPosition = new Short3(x, y, z);

                            bool buildShape = false;

                            switch (generationType) {
                                case GenerationType.sphere:
                                    buildShape = (gridPosition.magnitude < generatorRadius && gridPosition.magnitude > generatorRadius * (1 - generationThickness));
                                    break;
                                case GenerationType.cube:
                                    //buildShape = (Mathf.Abs(gridPosition.x) > generatorRadius * (1 - generationThickness) || Mathf.Abs(gridPosition.y) > generatorRadius * (1 - generationThickness) || Mathf.Abs(gridPosition.z) > generatorRadius * (1 - generationThickness));
                                    break;
                                default:
                                    break;
                            }

                            if (buildShape) {
                                grid.AddObject(new DefautShape(), gridPosition);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Public Methods
        public void SerializeGridToFile()
        {
            baker.SerializeGridToFile(folderPath, fileName);
        }
        #endregion
    }
}
