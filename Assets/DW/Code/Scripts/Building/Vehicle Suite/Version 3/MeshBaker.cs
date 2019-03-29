using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Fishy.Grids;
using Fishy.Meshes;
using Fishy.DataTypes;
using Fishy.Serialization;

namespace DW.Building.VehicleSuite3
{
	public class MeshBaker {
        #region Variables
        //Public & Serialized

        //Private
        private VehicleBuilder vehicleBuilder;
        private const int vertLimit = 60000;
        private const string gridFileExtension = ".fshGrid";

        private Dictionary<int, GameObject> meshObjectDictionary = new Dictionary<int, GameObject>();
        private GameObject selectedObject;

        private DefautShape defaultShape = new DefautShape();
        #endregion;

        #region Constructor
        public MeshBaker(VehicleBuilder vehicleBuilder)
        {
            this.vehicleBuilder = vehicleBuilder;
            selectedObject = new GameObject("Selected");
            selectedObject.transform.parent = vehicleBuilder.transform;
            selectedObject.AddComponent<MeshRenderer>();
            selectedObject.AddComponent<MeshFilter>();
            selectedObject.AddComponent<MeshCollider>();
        }
        #endregion

        #region Properties

        #endregion;

        #region Custom Methods
        public GameObject GeneratePointerShape()
        {
            GameObject meshObject = new GameObject("Pointer");
            meshObject.transform.SetParent(vehicleBuilder.transform);
            MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
            UpdatePointerMesh(meshObject, Short3.zero);
            meshObject.transform.GetComponent<Renderer>().material = vehicleBuilder.ghostMaterial;
            return meshObject;
        }

        public void UpdatePointerMesh(GameObject pointer, Volume volume)
        {
            MeshBuilder meshBuilder = new MeshBuilder();
            Vector3 worldPosition = volume.Center * vehicleBuilder.gridScale;
            Vector3 scale = new Vector3(volume.Width, volume.Height, volume.Depth) * vehicleBuilder.gridScale;
            defaultShape.PassThroughMeshBuilder(meshBuilder, worldPosition, scale);

            MeshFilter meshFilter = pointer.GetComponent<MeshFilter>();
            if (meshFilter) meshFilter.sharedMesh = meshBuilder.CreateMesh();
        }

        public void BakeToMesh(List<IShape> selected, float speed = 0.5f)
        {
            float timeStart = Time.realtimeSinceStartup;

            List<MeshBuilder> meshBuilders = new List<MeshBuilder>();
            MeshBuilder meshBuilder = new MeshBuilder();
            MeshBuilder meshBuilderSelected = new MeshBuilder(); //For any shapes that are selected

            Grid3D<IShape> grid = vehicleBuilder.Grid;
            IShape[] shapes = grid.GetObjects().ToArray();

            float unoptimizedVertCount = shapes.Length * 24;
            float bakedVertCount = 0;

            //Loop through all shapes and add them to a meshBuilder
            for (int i = 0; i < shapes.Length; i++) {
                Volume volume = grid.GetVolume(shapes[i]);
                Vector3 worldPosition = volume.Center * vehicleBuilder.gridScale;
                Vector3 scale = new Vector3(volume.Width, volume.Height, volume.Depth) * vehicleBuilder.gridScale;

                if (selected.Contains(shapes[i])) {
                    shapes[i].PassThroughMeshBuilder(meshBuilderSelected, worldPosition, scale);
                } else {
                    if (speed < 0.2f) shapes[i].CalculateNeighbors(grid);
                    shapes[i].PassThroughMeshBuilder(meshBuilder, worldPosition, scale);

                    //If the meshBuilder is getting too big, make a new one
                    if (meshBuilder.Vertices.Count > vertLimit) {
                        meshBuilders.Add(meshBuilder);
                        meshBuilder = new MeshBuilder();
                    }
                }
            }

            meshBuilders.Add(meshBuilder);

            //Build the selected mesh
            Mesh selectedMesh = meshBuilderSelected.CreateMesh();
            selectedObject.GetComponent<MeshFilter>().sharedMesh = selectedMesh;
            selectedObject.GetComponent<MeshCollider>().sharedMesh = selectedMesh;
            selectedObject.GetComponent<Renderer>().material = vehicleBuilder.selectedMaterial;

            //Go through all the builders and update
            for (int i = 0; i < meshBuilders.Count; i++) {
                Mesh mesh = meshBuilders[i].CreateMesh();
                bakedVertCount += mesh.vertexCount;
                UpdateMeshObject(i, mesh, vehicleBuilder.mainMaterial);
            }

            //Remove any extra meshes from past bakes
            for (int i = meshBuilders.Count; i < meshObjectDictionary.Count; i++) {
                UpdateMeshObject(i, null, vehicleBuilder.mainMaterial);
            }

            vehicleBuilder.meshCompressionRate = (int)(100 - ((bakedVertCount / unoptimizedVertCount) * 100f));

            //Debug.Log("baked " + shapes.Length + "  shapes in " + (Time.realtimeSinceStartup - timeStart) + " seconds");
        }

        public void SerializeGridToFile(string folderPath, string fileName)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string dataPath = Path.Combine(folderPath, fileName + gridFileExtension);
            BinaryFormatter binaryFormatter = Serialzation.GenerateBinaryFormatter();

            using (FileStream fileStream = File.Open(dataPath, FileMode.OpenOrCreate)) {
                binaryFormatter.Serialize(fileStream, vehicleBuilder.Grid);
            }
        }

        //Allocates meshes to objects
        private void UpdateMeshObject(int index, Mesh mesh, Material material)
        {
            GameObject meshObject;
            MeshRenderer meshRenderer;
            MeshFilter meshFilter;
            MeshCollider meshCollider;

            if (meshObjectDictionary.TryGetValue(index, out meshObject)) {
                meshFilter = meshObject.GetComponent<MeshFilter>();
                meshCollider = meshObject.GetComponent<MeshCollider>();
            }
            else {
                meshObject = new GameObject("Mesh " + index);
                meshObject.transform.parent = vehicleBuilder.transform;
                meshRenderer = meshObject.AddComponent<MeshRenderer>();
                meshFilter = meshObject.AddComponent<MeshFilter>();
                meshCollider = meshObject.AddComponent<MeshCollider>();

                meshObjectDictionary.Add(index, meshObject);
            }

            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
            meshObject.transform.GetComponent<Renderer>().material = material;
        }
        #endregion
    }
}
