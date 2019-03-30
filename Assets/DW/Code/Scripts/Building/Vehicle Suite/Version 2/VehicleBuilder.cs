using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unifish.GridsOld;
using Unifish.Meshes;
using Unifish.Serialization;

namespace DW.Building.LegacyBuildingSuite {
    public class VehicleBuilder : MonoBehaviour {
        #region Variables
        //Public & Serialized
        public int gridSize = 8;
        public float gridScale = 0.3f;
        public int generatorRadius = 6;
        public float randomGeneratorVolume = 0.02f;
        public float generationThickness = 0.2f;
        public Vector3 gridCentre = Vector3.zero;
        public GenerationType generationType = GenerationType.sphere;

        public Material mainMaterial;
        public Material ghostMaterial;
        public Material selectedMaterial;

        public float cameraRange = 20f;
        public EditMode editMode = EditMode.select;
        public KeyCode keyBake = KeyCode.B;
        public KeyCode keyCycleMode = KeyCode.F;

        public string directoryName = "Vehicle Builder";
        public string fileName = "default";

        public int vertsPerMesh = 10000;

        //Private
        
        private Grid3D<DynamicShape> grid;
        private string folderPath;
        private const string fileExtension = ".dat";
        private int gridArea;

        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        private MeshRenderer meshRenderer;
        private MeshBaker meshBaker;

        private List<GameObject> monoShapes;
        private Dictionary<DynamicShape, GameObject> monoShapeDictionary;
        private Mesh genericCubeMesh;

        private GameObject ghostBlock;

        private Transform lastHit; // The transform which the raycast hit 
        private Vector3 gridNormal; //The grid in which the raycast hit
        private Vector3 gridHit; //The grid which caused the raycast to collide

        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        private void Awake()
        {
            
            folderPath = Path.Combine(Application.persistentDataPath, directoryName);

            meshBaker = new MeshBaker(this);
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();

            //GetComponent<Renderer>().material = mainMaterial;

            DynamicShape ghostShape = new DynamicShape(Vector3.zero, Vector3.one * gridScale);
            genericCubeMesh = ghostShape.BuildMesh();
            ghostBlock = new GameObject("Ghost Block");
            ghostBlock.transform.parent = transform;
            MonoShape monoShape = ghostBlock.AddComponent<MonoShape>();
            monoShape.hasCollider = false;
            monoShape.GenerateWorldShape(ghostShape, genericCubeMesh);

            if (ghostMaterial) {
                Renderer renderer = ghostBlock.GetComponent<Renderer>();
                if (renderer) {
                    renderer.material = ghostMaterial;
                }
            }
        }

        private void Start()
        {
            if (!LoadGrid(fileName)) {
                //Couldnt load a grid, so generate a random one instead
                Debug.Log("Generating new Grid");
                grid = new Grid3D<DynamicShape>(gridCentre, gridSize, gridScale);
                DynamicShape dynamicShape;

                switch (generationType) {
                    case GenerationType.sphere:
                        for (int x = -grid.Size; x < grid.Size; x++) {
                            for (int y = -grid.Size; y < grid.Size; y++) {
                                for (int z = -grid.Size; z < grid.Size; z++) {

                                    if (Random.Range(0f, 1f) <= randomGeneratorVolume) {

                                        Vector3 gridPosition = new Vector3(x, y, z);

                                        if (gridPosition.magnitude < generatorRadius & gridPosition.magnitude > generatorRadius * (1- generationThickness)) {
                                            dynamicShape = new DynamicShape(grid.GridToWorldPos(gridPosition), Vector3.one * grid.Scale);
                                            grid.AddObject(dynamicShape);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case GenerationType.cube:
                        for (int x = -grid.Size; x < grid.Size; x++) {
                            for (int y = -grid.Size; y < grid.Size; y++) {
                                for (int z = -grid.Size; z < grid.Size; z++) {

                                    if (Random.Range(0f, 1f) <= randomGeneratorVolume) {

                                        Vector3 gridPosition = new Vector3(x, y, z);

                                        if (Mathf.Abs(gridPosition.x) > generatorRadius * (1 - generationThickness) || Mathf.Abs(gridPosition.y) > generatorRadius * (1 - generationThickness) || Mathf.Abs(gridPosition.z) > generatorRadius * (1 - generationThickness)) {
                                            dynamicShape = new DynamicShape(grid.GridToWorldPos(gridPosition), Vector3.one * grid.Scale);
                                            grid.AddObject(dynamicShape);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case GenerationType.single:
                        dynamicShape = new DynamicShape(Vector3.zero, Vector3.one * grid.Scale);
                        grid.AddObject(dynamicShape);
                        break;
                    default:
                        break;
                }
            } else {
                Debug.Log("Loaded grid from file: scale = " + grid.Scale);
                grid.Scale = gridScale;
                gridSize = grid.Size;
            }

            //GenerateGridShapes();
            BakeMesh(0);
            //grid.PrintObjectsToLog();
        }

        private void Update()
        {
            HandleInput();
            HandleRaycast();
        }
        #endregion;

        #region MonoShape Methods (DEPRECIATED)
        /// <summary>
        /// Creates a real world version of a dynamicShape, using its position from grid.
        /// </summary>
        /// <param name="dynamicShape"></param>
        private GameObject GenerateMonoShape(DynamicShape dynamicShape)
        {
            GameObject shapeObject = new GameObject("Shape");
            Vector3 worldPos = dynamicShape.WorldPosition;
            //shapeObject.transform.position = worldPos;
            shapeObject.transform.parent = transform;
            MonoShape monoShape = shapeObject.AddComponent<MonoShape>();
            monoShape.GenerateWorldShape(dynamicShape);

            if (mainMaterial) {
                Renderer renderer = shapeObject.GetComponent<Renderer>();
                if (renderer) {
                    renderer.material = mainMaterial;
                }
            }

            monoShapes.Add(shapeObject);
            monoShapeDictionary.Add(dynamicShape, shapeObject);

            return shapeObject;
        }

        /// <summary>
        /// Builds fresh shapes from scrath
        /// </summary>
        public void GenerateGridShapes()
        {
            //Clear old shapes
            ClearMonoShapes();

            //Now construct each shape
            foreach (var dynamicShape in grid.GetGridObjects()) {
                GenerateMonoShape(dynamicShape);
            }
        }

        public void ClearMonoShapes()
        {
            if (monoShapes != null) {
                foreach (var shape in monoShapes) {
                    Destroy(shape);
                }
            }
            monoShapes = new List<GameObject>();
            monoShapeDictionary = new Dictionary<DynamicShape, GameObject>();
        }
        #endregion

        #region Mesh Methods
        public void BakeMesh(float priority = 0.5f)
        {
            meshBaker.BakeMesh(grid, priority);
            ClearMonoShapes();
        }

        public void SaveGrid()
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string dataPath = Path.Combine(folderPath, fileName + fileExtension);
            BinaryFormatter binaryFormatter = Serialzation.GenerateBinaryFormatter();

            using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate)) {
                binaryFormatter.Serialize(fileStream, grid);
            }
        }

        public bool LoadGrid(string file)
        {

            if (!Directory.Exists(folderPath)) return false;

            string dataPath = Path.Combine(folderPath, file + fileExtension);
            string[] filePaths = Directory.GetFiles(folderPath);

            foreach (var curentFile in filePaths) {
                if (curentFile == dataPath) {
                    BinaryFormatter binaryFormatter = Serialzation.GenerateBinaryFormatter();

                    using (FileStream fileStream = File.Open(curentFile, FileMode.Open)) {
                        grid = (Grid3D<DynamicShape>)binaryFormatter.Deserialize(fileStream);
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region Input Management
        private void HandleInput()
        {
            if (Input.GetKeyDown(keyBake)) BakeMesh(0);
            if (Input.GetKeyDown(keyCycleMode)) CycleEditMode();
            if (Input.GetKeyDown(KeyCode.Alpha1)) CycleEditMode(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) CycleEditMode(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) CycleEditMode(2);

            if (Input.GetMouseButtonDown(0)) {
                switch (editMode) {
                    case EditMode.select:
                        Debug.Log(gridHit);
                        break;
                    case EditMode.build:
                        if (lastHit != null && gridNormal != null) {
                            //Create a new shape
                            DynamicShape dynamicShape = new DynamicShape(grid.GridToWorldPos(gridNormal), Vector3.one * gridScale);
                            if (grid.AddObject(dynamicShape)) {
                                //Grid accepted new shape- spawn it in real world
                                //GenerateMonoShape(dynamicShape);
                                BakeMesh(1);
                            } else {
                                Debug.LogError("cant place block at " + gridNormal + " - grid rejected it");
                            }
                        }
                        break;
                    case EditMode.deform:
                        break;
                    default:
                        break;
                }
            }
            if (Input.GetMouseButtonDown(1)) {
                switch (editMode) {
                    case EditMode.select:
                        break;
                    case EditMode.build:
                        DynamicShape shape = grid.GetObjectAtGrid(gridHit);
                        if(shape != null) {
                            grid.RemoveObject(shape);
                            BakeMesh(0.5f); // 1 = fast!
                        }
                        break;
                    case EditMode.deform:
                        break;
                    default:
                        break;
                }
            }
        }

        private void HandleRaycast()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (UnityEngine.Physics.Raycast(ray, out hit, cameraRange)) {

                //If this is a new selection, reset the old target
                if (hit.transform != lastHit) {
                    CancelRaycastTarget();
                    lastHit = hit.transform;
                    if (editMode == EditMode.build) {
                        ghostBlock.SetActive(true);
                    } else if (editMode == EditMode.select) {
                        SetMaterial(lastHit, selectedMaterial);
                    }
                    CalculateNormalHit(hit);
                } else {
                    CalculateNormalHit(hit);
                }
            } else {
                CancelRaycastTarget();
            }
        }

        private void CalculateNormalHit(RaycastHit hit)
        {
            gridHit = grid.GetNearestObjectGridPosition(hit.point);
            gridNormal = gridHit + hit.normal;
            ghostBlock.transform.position = grid.GridToWorldPos(gridNormal);
        }
        private void CancelRaycastTarget()
        {
            if (lastHit) {
                SetMaterial(lastHit, mainMaterial);
                lastHit = null;
                ghostBlock.SetActive(false);
            }
        }

        private void CycleEditMode(int mode = -1)
        {
            editMode = (mode == -1) ? editMode + 1 : (EditMode)mode;
            if ((int)editMode > 2) {
                editMode = 0;
            }
            CancelRaycastTarget();
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
    }
}
