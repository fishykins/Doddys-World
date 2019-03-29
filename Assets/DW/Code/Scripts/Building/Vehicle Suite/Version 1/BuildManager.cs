using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fishy.GridsOld;
using Fishy.DataTypes;

namespace DW.Building.DepreciatedShipSuite
{
	public class BuildManager : MonoBehaviour {
        #region Variables
        //Public & Serialized
        [Header("Build Settings")]
        [SerializeField, Range(0.5f, 4f)]
        private float blockScale = 2;
        [SerializeField, Range(4, 32)]
        private int gridSize = 16;

        [Header("Prefabs")]
        [SerializeField]
        private GameObject standardShape = null;
        [SerializeField]
        private Material standardMaterial = null;

        //Private
        private ShipManager shipManager;
        private InputManager inputManager;
        private Grid3D<DynamicShape> grid;
        #endregion;

        #region Properties
        public float BlockScale { get { return blockScale; } }
        public Grid3D<DynamicShape> Grid { get { return grid; } }
        public GameObject StandardShape { get { return standardShape; } }
        public Material StandardMaterial { get { return standardMaterial; } }
        public Vector3 DefaultScale { get { return Vector3.one * BlockScale; } }
        #endregion;


        #region Unity Methods
        private void Awake()
        {
            shipManager = GetComponent<ShipManager>();
            inputManager = GetComponent<InputManager>();
            grid = new Grid3D<DynamicShape>(Vector3.zero, gridSize, blockScale);
        }

        private void Start()
        {
            GenerateCube(Vector3.zero, DefaultScale);
        }

        private void OnValidate()
        {
            blockScale *= 2f;
            blockScale = Mathf.RoundToInt(blockScale) / 2f;
        }
        #endregion;

        #region Custom Methods
        public DynamicShape CombineCubes(List<DynamicShape> shapes)
        {
            if (shapes.Count < 2) return null;

            List<Vector3> vectors = new List<Vector3>();
            for (int i = 0; i < shapes.Count; i++) {
                Short3[] gridArray = grid.GetObjectGridPosition(shapes[i]);
                //vectors.AddRange(gridArray);
            }

            if (grid.GetAreaFill(vectors) < 1) return null; //Cant combine because they do not make up a cube

            //Combine (or destroy old)!
            foreach (DynamicShape shape in shapes) DestroyShape(shape);
            //Build single cube
            GridData data = new GridData(vectors);
            Vector3 spawnPos = grid.GridToWorldPos(data.average);
            Vector3 spawnScale = (Vector3.one + data.max - data.min) * blockScale;
            Debug.Log("spawnScale = " + spawnScale);
            
            DynamicShape combinedCube = GenerateCube(spawnPos, spawnScale, true); //Bypass built-in grid check- we know that its free!

            if (!combinedCube) return null; //This shouldnt happen, but who knows

            //grid.AddObject(combinedCube, vectors.ToArray()); //Manually occupy the grid

            return combinedCube;
        }

        public void SplitCubes(DynamicShape shape)
        {

        }

        public DynamicShape GenerateCube(Vector3 worldPosition, Vector3 scale, bool bypassCheck = false)
        {
            if (!standardShape) return null;

            GameObject obj = Instantiate(standardShape);
            obj.transform.parent = transform;
            obj.transform.position = worldPosition;
            obj.transform.name = "Cube [" + (int)grid.WorldToGridPos(worldPosition).x + "," + (int)grid.WorldToGridPos(worldPosition).y + "," + (int)grid.WorldToGridPos(worldPosition).z + "]";

            DynamicShape shape = obj.GetComponent<DynamicShape>();

            if (shape) {

                if (!bypassCheck) {
                    //Not allowed to bypass grid check, so see if position is free
                    bypassCheck = grid.AddObjectAtWorldPosition(shape, worldPosition);
                }

                if (bypassCheck) {
                    shape.SetMaterial(standardMaterial);
                    shape.BuildBasicCube(worldPosition, scale);
                    return shape;
                } else {
                    Destroy(obj);
                    Debug.LogError("Grid position occupied- why was this object spawned???");
                }
            } else {
                Destroy(obj);
                Debug.LogError("No shape found on prefab " + standardShape.name);
            }

            return null;
        }

        public void DestroyShape(DynamicShape shape)
        {
            grid.RemoveObject(shape);
            Destroy(shape.gameObject);
        }
        #endregion
    }
}
