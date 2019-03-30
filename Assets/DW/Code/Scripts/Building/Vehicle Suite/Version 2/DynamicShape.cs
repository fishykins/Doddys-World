using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unifish.GridsOld;
using Unifish.Meshes;

namespace DW.Building.LegacyBuildingSuite {
    [System.Serializable]
	public class DynamicShape : IGridObject {
        #region Variables
        //Public & Serialized
        public Vector3 worldPosition;

        public DynamicVertex vertexNone = new DynamicVertex();
        public DynamicVertex upForwardRight = new DynamicVertex();
        public DynamicVertex upForwardLeft = new DynamicVertex();
        public DynamicVertex upBackwardRight = new DynamicVertex();
        public DynamicVertex upBackwardLeft = new DynamicVertex();
        public DynamicVertex downForwardRight = new DynamicVertex();
        public DynamicVertex downForwardLeft = new DynamicVertex();
        public DynamicVertex downBackwardRight = new DynamicVertex();
        public DynamicVertex downBackwardLeft = new DynamicVertex();

        private DynamicVertex[] Verticies = new DynamicVertex[8];

        public DynamicQuad top;
        public DynamicQuad bottom;
        public DynamicQuad front;
        public DynamicQuad back;
        public DynamicQuad left;
        public DynamicQuad right;

        #endregion;

        #region Properties
        public Vector3 WorldPosition { get { return worldPosition; } set { worldPosition = value; } }
        #endregion;

        #region Constructor
        public DynamicShape(Vector3 worldPosition, Vector3 scale)
        {
            this.worldPosition = worldPosition;
            BuildVertexPoints(scale);
            BuildCubeFaces();
        }
        #endregion;

        #region Custom Methods
        public void BuildVertexPoints(Vector3 scale)
        {
            float x = scale.x / 2f;
            float y = scale.y / 2f;
            float z = scale.z / 2f;

            //offset = new Vector3(offset.x * scale.x, offset.y * scale.y, offset.z * scale.z);

            upForwardRight.point = worldPosition + new Vector3(x, y, z);
            upForwardLeft.point = worldPosition + new Vector3(-x, y, z);
            upBackwardRight.point = worldPosition + new Vector3(x, y, -z);
            upBackwardLeft.point = worldPosition + new Vector3(-x, y, -z);
            downForwardRight.point = worldPosition + new Vector3(x, -y, z);
            downForwardLeft.point = worldPosition + new Vector3(-x, -y, z);
            downBackwardRight.point = worldPosition + new Vector3(x, -y, -z);
            downBackwardLeft.point = worldPosition + new Vector3(-x, -y, -z);
        }

        public void BuildCubeFaces()
        {
            top = new DynamicQuad(upForwardRight, upBackwardRight, upBackwardLeft, upForwardLeft);
            bottom = new DynamicQuad(downBackwardLeft, downBackwardRight, downForwardRight, downForwardLeft);
            front = new DynamicQuad(upForwardRight, upForwardLeft, downForwardLeft, downForwardRight);
            back = new DynamicQuad(downBackwardLeft, upBackwardLeft, upBackwardRight, downBackwardRight);
            left = new DynamicQuad(downBackwardLeft, downForwardLeft, upForwardLeft, upBackwardLeft);
            right = new DynamicQuad(upForwardRight, downForwardRight, downBackwardRight, upBackwardRight);
        }

        public Mesh BuildMesh()
        {
            MeshBuilder builder = new MeshBuilder();
            PassThroughMeshBuilder(builder);
            return builder.CreateMesh();
        }

        public void PassThroughMeshBuilder(MeshBuilder builder)
        {
            top.BuildQuad(builder);
            bottom.BuildQuad(builder);
            front.BuildQuad(builder);
            back.BuildQuad(builder);
            left.BuildQuad(builder);
            right.BuildQuad(builder);
        }

        public override string ToString()
        {
            return "dynamicShape";
        }
        #endregion
	}
}
