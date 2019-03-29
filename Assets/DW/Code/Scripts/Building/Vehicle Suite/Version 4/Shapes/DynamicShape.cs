using System.Collections;
using System.Collections.Generic;
using Fishy.DataTypes;
using UnityEngine;

namespace DW.Building.VehicleSuite4 {
    //Everything in a shape works on local scale- it knows nothing of the real world
    public class DynamicShape 
    {
        #region Variables
        //Public & Serialized
        public Vector3[] localVerts;

        #endregion;

        #region Constructor
        public DynamicShape()
        {
            localVerts = CreateVertexArray();
        }
        #endregion

        #region Custom Methods
        public Vector3[][] GetQuads()
        {
            Vector3[] top = new Vector3[] { localVerts[1], localVerts[3], localVerts[2], localVerts[0] };
            Vector3[] bottom = new Vector3[] { localVerts[6], localVerts[7], localVerts[5], localVerts[4] };
            Vector3[] left = new Vector3[] { localVerts[6], localVerts[4], localVerts[0], localVerts[2] };
            Vector3[] right = new Vector3[] { localVerts[1], localVerts[5], localVerts[6], localVerts[3] };
            Vector3[] front = new Vector3[] { localVerts[1], localVerts[0], localVerts[4], localVerts[5] };
            Vector3[] back = new Vector3[] { localVerts[6], localVerts[2], localVerts[3], localVerts[7] };

            /*
            if (quads[0]) BuildQuad(builder, topFrontRight, topBackRight, topBackLeft, topFrontLeft); //Top
            if (quads[1]) BuildQuad(builder, bottomBackleft, bottomBackRight, bottomFrontRight, bottomFrontLeft); //Bottom
            if (quads[2]) BuildQuad(builder, bottomBackleft, bottomFrontLeft, topFrontLeft, topBackLeft); //Left
            if (quads[3]) BuildQuad(builder, topFrontRight, bottomFrontRight, bottomBackRight, topBackRight); //Right
            if (quads[4]) BuildQuad(builder, topFrontRight, topFrontLeft, bottomFrontLeft, bottomFrontRight); //Front
            if (quads[5]) BuildQuad(builder, bottomBackleft, topBackLeft, topBackRight, bottomBackRight); //back 
            */

            return new Vector3[][] { top, bottom, left, right, front, back };
        }

        public void Deform(Vector3 start, Vector3 end)
        {
            Vector3[] startPoints = CreateVertexArray();
            int index = GetNearestIndex(start, startPoints);
            if (index >= 0) {
                localVerts[index] = end;
            }
        }

        private int GetNearestIndex(Vector3 point, Vector3[] array)
        {
            float nearestDist = float.MaxValue;
            int nearestPoint = -1;

            for (int i = 0; i < array.Length; i++) {
                if ((point - array[i]).sqrMagnitude < nearestDist) {
                    nearestDist = (point - array[i]).sqrMagnitude;
                    nearestPoint = i;
                }
            }
            return nearestPoint;
        }

        private Vector3[] CreateVertexArray()
        {
            Vector3 topFrontLeft = new Vector3(-1, 1, 1); //0
            Vector3 topFrontRight = new Vector3(1, 1, 1); //1
            Vector3 topBackLeft = new Vector3(-1, 1, -1); //2
            Vector3 topBackRight = new Vector3(1, 1, -1); //3
            Vector3 bottomFrontLeft = new Vector3(-1, -1, 1); //4
            Vector3 bottomFrontRight = new Vector3(1, -1, 1); //5
            Vector3 bottomBackleft = new Vector3(-1, -1, -1); //6
            Vector3 bottomBackRight = new Vector3(1, 1, -1); //7

            return new Vector3[] { topFrontLeft, topFrontRight, topBackLeft, topBackRight, bottomFrontLeft, bottomFrontRight, bottomBackleft, bottomBackRight };
        }
        #endregion

    }
}
