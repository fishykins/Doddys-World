using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unifish.GridsOld;
using Unifish.Meshes;

namespace DW.Building.LegacyBuildingSuite {
    //Bakes a collecton of DynamicShapes into meshes
	public class MeshBaker {
        #region Variables
        //Public & Serialized

        //Private
        private const int maxVerts = 60000;

        private Grid3D<DynamicShape> grid;
        private DynamicShape[] dynamicShapes;
        private MeshBuilder builder;
        private VehicleBuilder vehicleBuilder;

        private Vector3 nUp = new Vector3(0, 1, 0);
        private Vector3 nDown = new Vector3(0, -1, 0);
        private Vector3 nLeft = new Vector3(-1, 0, 0);
        private Vector3 nRight = new Vector3(1, 0, 0);
        private Vector3 nFront = new Vector3(0, 0, 1);
        private Vector3 nBack = new Vector3(0, 0, -1);

        private Dictionary<int, GameObject> MeshObjectDictionary = new Dictionary<int, GameObject>();
        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        public MeshBaker(VehicleBuilder vehicleBuilder)
        {
            this.vehicleBuilder = vehicleBuilder;
        }
		#endregion;

        #region Custom Methods
        //Bakes a mesh. priority 1 is the fastest but least optimized
        public void BakeMesh(Grid3D<DynamicShape> grid, float priority = 0)
        {
            this.grid = grid;
            dynamicShapes = grid.GetGridObjects();

            //Establish the maximum number of meshes we are going to need
            float vertLimmit = Mathf.Min(maxVerts, vehicleBuilder.vertsPerMesh);
            float rawVertCount = dynamicShapes.Length * 24;
            int meshCount = 1 + Mathf.FloorToInt(rawVertCount / vertLimmit);

            //Now build the MeshBuilders (one for each mesh)
            MeshBuilder[] meshBuilders = new MeshBuilder[meshCount];

            for (int i = 0; i < meshCount; i++) {
                meshBuilders[i] = new MeshBuilder();
            }

            //Run through each DynamicShape and execute bake
            float shapesPerMesh = 1f + dynamicShapes.Length / meshCount;

            int currentMeshBuilder = 0;

            for (int i = 0; i < dynamicShapes.Length; i++) {
                DynamicShape shape = dynamicShapes[i];
                MeshBuilder meshBuilder = meshBuilders[currentMeshBuilder];

                if (priority < 0.6f) 
                    CheckNeighbors(shape);

                shape.PassThroughMeshBuilder(meshBuilder);

                if (meshBuilder.Vertices.Count > vertLimmit) {
                    currentMeshBuilder++;
                    //Debug.Log("New meshBuilder (" + currentMeshBuilder + ") at " + meshBuilder.Vertices.Count + " verts");
                }
            }

            //Build each mesh
            int totalVertCount = 0;

            for (int i = 0; i <= currentMeshBuilder; i++) {
                Mesh mesh = meshBuilders[i].CreateMesh();

                UpdateMeshObject(i, mesh);

                totalVertCount += mesh.vertexCount;
            }

            //Remove any extra meshes that have not been used
            CullExtraMeshes(currentMeshBuilder + 1);

            if (priority <= 0.1f) {
                float compressionRate = 100 - (totalVertCount / rawVertCount) * 100f;
                Debug.Log("Baked " + dynamicShapes.Length + " shapes with " + compressionRate + "% compression (" + totalVertCount + " verts) over " + (1 + currentMeshBuilder) + " mesh");
            }
        }

        private void CullExtraMeshes(int maxIndex)
        {
            for (int i = maxIndex; i < MeshObjectDictionary.Count; i++) {
                var item = MeshObjectDictionary[i];
                UpdateMeshObject(i, null);
            }
        }

        private void UpdateMeshObject(int index, Mesh mesh)
        {
            GameObject meshObject;
            MeshRenderer meshRenderer;
            MeshFilter meshFilter;
            MeshCollider meshCollider;

            if (MeshObjectDictionary.TryGetValue(index, out meshObject)) {
                meshFilter = meshObject.GetComponent<MeshFilter>();
                meshCollider = meshObject.GetComponent<MeshCollider>();
            } else {
                meshObject = new GameObject("Mesh " + index);
                meshObject.transform.parent = vehicleBuilder.transform;
                meshRenderer = meshObject.AddComponent<MeshRenderer>();
                meshFilter = meshObject.AddComponent<MeshFilter>();
                meshCollider = meshObject.AddComponent<MeshCollider>();

                MeshObjectDictionary.Add(index, meshObject);
            }
            
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
            meshObject.transform.GetComponent<Renderer>().material = vehicleBuilder.mainMaterial;
        }


        /// <summary>
        /// Bakes a mesh and spawns it under the VehicleBuilder
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public void BakeDemoMesh(Grid3D<DynamicShape> grid)
        {
            this.grid = grid;
            this.dynamicShapes = grid.GetGridObjects();
            this.builder = new MeshBuilder();

            float rawVertCount = dynamicShapes.Length * 24;

            if (rawVertCount > maxVerts) {
                Debug.LogWarning("This shape exceeds the vertex limit- we will probably need to split the mesh");
            }

            for (int i = 0; i < dynamicShapes.Length; i++) {
                DynamicShape shape = dynamicShapes[i];
                CheckNeighbors(shape);
                //ActivateAllFaces(shape);
                shape.PassThroughMeshBuilder(builder);
            }

            Mesh mesh = builder.CreateMesh();
            mesh.RecalculateNormals();
    
            float compressionRate = 100 - (mesh.vertexCount / rawVertCount) * 100f;
            Debug.Log("Mesh baked " + dynamicShapes.Length + " shapes with " + compressionRate + "% compression (" + mesh.vertexCount + " verts)");

        }


        private void ActivateAllFaces(DynamicShape shape)
        {
            shape.top.active = true;
            shape.bottom.active = true;
            shape.left.active = true;
            shape.right.active = true;
            shape.front.active = true;
            shape.back.active = true;
        }

        private void CheckNeighbors(DynamicShape shape)
        {
            Vector3 gridPosition = grid.GetObjectGridPosition(shape)[0];

            //up
            shape.top.active = (grid.GetObjectAtGrid(gridPosition + nUp) == null);

            //down
            shape.bottom.active = (grid.GetObjectAtGrid(gridPosition + nDown) == null);

            //left
            shape.left.active = (grid.GetObjectAtGrid(gridPosition + nLeft) == null);

            //right
            shape.right.active = (grid.GetObjectAtGrid(gridPosition + nRight) == null);

            //front
            shape.front.active = (grid.GetObjectAtGrid(gridPosition + nFront) == null);

            //back
            shape.back.active = (grid.GetObjectAtGrid(gridPosition + nBack) == null);
        }
        #endregion
	}
}
