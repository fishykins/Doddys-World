using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unifish.DataTypes;
using Unifish.Grids;
using Unifish.Meshes;
using DW.Building;

namespace DW.Building.VehicleSuite4 {
	public class MeshManager : MonoBehaviour {
        #region Variables
        //Public & Serialized

        //Private
        private VehicleManager vehicleManager;
        private GameObject lastMeshObject;
        private GameObject ghostMeshObject;

        private Volume bakedSelection = new Volume(Short3.zero);
		#endregion;

		#region Properties

		#endregion;

		#region Unity Methods
		private void Awake () {
            vehicleManager = GetComponent<VehicleManager>();

        }

		void Update () {
			
		}
        #endregion;

        #region Custom Methods
        public void BakeGeometry()
        {
            MeshBuilder builder = new MeshBuilder();
            Grid3D<IShape> grid = vehicleManager.geometry;
            IShape[] shapes = grid.GetObjects().ToArray();

            for (int i = 0; i < shapes.Length; i++) {
                Volume volume = grid.GetVolume(shapes[i]);
                Quad[] quads = shapes[i].GetQuads(volume, vehicleManager.gridCentre, vehicleManager.gridScale);

                for (int j = 0; j < quads.Length; j++) {
                    Quad quad = quads[j];
                    builder.AddQuad(quad);
                }
            }

            Mesh mesh = builder.CreateMesh();
            if (lastMeshObject) Destroy(lastMeshObject);
            lastMeshObject = BuildingUtil.GenerateMeshObject("mesh obj", transform, vehicleManager.mainMaterial, mesh, true);
        }

        public void ClearSelection()
        {
            if (ghostMeshObject) Destroy(ghostMeshObject);
        }

        public void BakeSelection(Volume selection)
        {
            if (bakedSelection == selection) return;
            MeshBuilder builder = new MeshBuilder();
            IShape cube = new Cuboid();
            Quad[] quads = cube.GetQuads(selection, vehicleManager.gridCentre, vehicleManager.gridScale);
            for (int j = 0; j < quads.Length; j++) {
                Quad quad = quads[j];
                builder.AddQuad(quad);
            }

            ClearSelection();

            Mesh mesh = builder.CreateMesh();
            ghostMeshObject = BuildingUtil.GenerateMeshObject("ghost obj", transform, vehicleManager.ghostMaterial, mesh, false);
        }
        #endregion
	}
}
