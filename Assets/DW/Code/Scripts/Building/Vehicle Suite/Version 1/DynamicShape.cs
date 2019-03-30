using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unifish.Meshes;
using Unifish.GridsOld;

namespace DW.Building.DepreciatedShipSuite
{
    [Serializable, RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshCollider))]
	public class DynamicShape : MonoBehaviour, IGridObject {
        #region Variables
        //Public & Serialized
        public bool initialized = false;

        //Private
        private Vector3 worldPosition;
        private DynamicVertex vertexNone = new DynamicVertex();
        private DynamicVertex upForwardRight = new DynamicVertex();
        private DynamicVertex upForwardLeft = new DynamicVertex();
        private DynamicVertex upBackwardRight = new DynamicVertex();
        private DynamicVertex upBackwardLeft = new DynamicVertex();
        private DynamicVertex downForwardRight = new DynamicVertex();
        private DynamicVertex downForwardLeft = new DynamicVertex();
        private DynamicVertex downBackwardRight = new DynamicVertex();
        private DynamicVertex downBackwardLeft = new DynamicVertex();

        private DynamicVertex[] Verticies = new DynamicVertex[8];

        private DynamicQuad top;
        private DynamicQuad bottom;
        private DynamicQuad front;
        private DynamicQuad back;
        private DynamicQuad left;
        private DynamicQuad right;

        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        #endregion;

        #region Properties
        public Vector3 WorldPosition { get { return worldPosition; } set { worldPosition = value; } }
        #endregion;

        #region Constructor
        public void BuildBasicCube(Vector3 worldPosition, Vector3 scale, bool hasCollider = true)
        {
            float x = scale.x / 2f;
            float y = scale.y / 2f;
            float z = scale.z / 2f;

            upForwardRight.point = new Vector3(x, y, z);
            upForwardLeft.point = new Vector3(-x, y, z);
            upBackwardRight.point = new Vector3(x, y, -z);
            upBackwardLeft.point = new Vector3(-x, y, -z);
            downForwardRight.point = new Vector3(x, -y, z);
            downForwardLeft.point = new Vector3(-x, -y, z);
            downBackwardRight.point = new Vector3(x, -y, -z);
            downBackwardLeft.point = new Vector3(-x, -y, -z);

            BuildCubeFaces();

            GenerateShape(hasCollider);

            initialized = true;
        }

        public void BuildBasicCube(Vector3 worldPosition, float scale, bool hasCollider = true)
        {
            float halfScale = scale / 2f;

            upForwardRight.point = new Vector3(halfScale, halfScale, halfScale);
            upForwardLeft.point = new Vector3(-halfScale, halfScale, halfScale);
            upBackwardRight.point = new Vector3(halfScale, halfScale, -halfScale);
            upBackwardLeft.point = new Vector3(-halfScale, halfScale, -halfScale);
            downForwardRight.point = new Vector3(halfScale, -halfScale, halfScale);
            downForwardLeft.point = new Vector3(-halfScale, -halfScale, halfScale);
            downBackwardRight.point = new Vector3(halfScale, -halfScale, -halfScale);
            downBackwardLeft.point = new Vector3(-halfScale, -halfScale, -halfScale);

            BuildCubeFaces();

            GenerateShape(hasCollider);

            initialized = true;
        }
        #endregion

        #region Unity Methods
        void Awake () {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();

            Verticies[0] = upForwardRight;
            Verticies[1] = upForwardLeft;
            Verticies[2] = upBackwardRight;
            Verticies[3] = upBackwardLeft;
            Verticies[4] = downForwardRight;
            Verticies[5] = downForwardLeft;
            Verticies[6] = downBackwardRight;
            Verticies[7] = downBackwardLeft;
        }
		#endregion;

        #region Custom Methods
        private void BuildCubeFaces()
        {
            top = new DynamicQuad(upForwardRight, upBackwardRight, upBackwardLeft, upForwardLeft);
            bottom = new DynamicQuad(downBackwardLeft, downBackwardRight, downForwardRight, downForwardLeft);
            front = new DynamicQuad(upForwardRight, upForwardLeft, downForwardLeft, downForwardRight);
            back = new DynamicQuad(downBackwardLeft, upBackwardLeft, upBackwardRight, downBackwardRight);
            left = new DynamicQuad(downBackwardLeft, downForwardLeft, upForwardLeft, upBackwardLeft);
            right = new DynamicQuad(upForwardRight, downForwardRight, downBackwardRight, upBackwardRight);
        }

        public ref DynamicVertex GetNearestVertex(Vector3 modelSpace)
        {
            float nearestDist = float.MaxValue;
            ref DynamicVertex v = ref upForwardRight;

            for (int i = 0; i < Verticies.Length; i++) {
                float dist = (modelSpace - Verticies[i].point).sqrMagnitude;
                if (dist < nearestDist) {
                    nearestDist = dist;
                    v = ref Verticies[i];
                }
            }

            if (nearestDist < 0.1f) {
                return ref v;
            } else {
                return ref vertexNone;
            }
        }

        public void GenerateShape(bool hasCollider = false)
        {
            MeshBuilder builder = new MeshBuilder();

            top.BuildQuad(builder);
            bottom.BuildQuad(builder);
            front.BuildQuad(builder);
            back.BuildQuad(builder);
            left.BuildQuad(builder);
            right.BuildQuad(builder);

            Mesh mesh = builder.CreateMesh();

            mesh.RecalculateNormals();

            if (meshFilter) {
                meshFilter.sharedMesh = mesh;
            }

            if (meshCollider && hasCollider) {
                if (!meshCollider.sharedMesh) {
                    meshCollider.sharedMesh = mesh;
                }
            }
        }

        public void SetMaterial(Material mat)
        {
            if (mat != null) {
                GetComponent<Renderer>().material = mat;
            }
        }
        #endregion
	}
}
