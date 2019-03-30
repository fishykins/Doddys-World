using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unifish.Meshes;

namespace DW.Building.LegacyBuildingSuite {
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshCollider)), RequireComponent(typeof(MeshRenderer))]
    public class MonoShape : MonoBehaviour {
        #region Variables
        //Public & Serialized
        public bool hasCollider = true;

        //Private
        private DynamicShape dynamicShape;
        private VehicleBuilder builder;
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        private Mesh mesh;
        #endregion;

        #region Properties
        public DynamicShape DynamicShape { get { return dynamicShape; } }
        #endregion;

        #region Unity Methods
        void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
        }
        #endregion;

        #region Custom Methods
        public void GenerateWorldShape(DynamicShape dynamicShape, Mesh mesh = null)
        {
            this.dynamicShape = dynamicShape;
            this.mesh = mesh;
            UpdateShape();
        }

        public void UpdateShape()
        {
            if (dynamicShape == null) {
                //No dynamicShape attached to us- we dont belong in this world anymore :(
                Destroy(this);
                return;
            }

            Mesh shapeMesh = (mesh != null) ? mesh: dynamicShape.BuildMesh();
            shapeMesh.RecalculateNormals();

            if (meshFilter) {
                meshFilter.sharedMesh = shapeMesh;
            }

            if (meshCollider && hasCollider) {
                meshCollider.sharedMesh = shapeMesh;
            }
        }
        #endregion
	}
}
