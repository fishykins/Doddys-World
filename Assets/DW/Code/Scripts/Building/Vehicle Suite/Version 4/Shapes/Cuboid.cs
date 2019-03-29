using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fishy.Meshes;
using Fishy.DataTypes;
using Fishy.Grids;

namespace DW.Building.VehicleSuite4
{ 
    public class Cuboid : IShape {

        public Quad[] GetQuads(Volume gridVolume, Vector3 worldAnchor, float gridScale)
        {
            Vector3[] localVerts = CreateVertexArray(gridVolume, worldAnchor, gridScale);
            Quad top = new Quad(localVerts[1], localVerts[3], localVerts[2], localVerts[0]);
            Quad bottom = new Quad(localVerts[6], localVerts[7], localVerts[5], localVerts[4]);
            Quad left = new Quad(localVerts[6], localVerts[4], localVerts[0], localVerts[2]);
            Quad right = new Quad(localVerts[1], localVerts[5], localVerts[7], localVerts[3]);
            Quad front = new Quad(localVerts[1], localVerts[0], localVerts[4], localVerts[5]);
            Quad back = new Quad(localVerts[6], localVerts[2], localVerts[3], localVerts[7]);

            return new Quad[] { top, bottom, left, right, front, back };
        }

        public void Deform(Vector3 start, Vector3 end)
        {
            //Do nothing, because we cant be deformed....
        }

        private Vector3[] CreateVertexArray(Volume gridVolume, Vector3 worldAnchor, float gridScale)
        {
            Vector3 center = BuildingUtil.GridToWorld(gridVolume.Center, worldAnchor, gridScale);
            float px = gridVolume.Width * gridScale / 2f;
            float py = gridVolume.Height * gridScale / 2f;
            float pz = gridVolume.Depth * gridScale / 2f;

            Vector3 topFrontLeft = center + new Vector3(-px, py, pz); //0
            Vector3 topFrontRight = center + new Vector3(px, py, pz); //1
            Vector3 topBackLeft = center + new Vector3(-px, py, -pz); //2
            Vector3 topBackRight = center + new Vector3(px, py, -pz); //3
            Vector3 bottomFrontLeft = center + new Vector3(-px, -py, pz); //4
            Vector3 bottomFrontRight = center + new Vector3(px, -py, pz); //5
            Vector3 bottomBackleft = center + new Vector3(-px, -py, -pz); //6
            Vector3 bottomBackRight = center + new Vector3(px, -py, -pz); //7

            return new Vector3[] { topFrontLeft, topFrontRight, topBackLeft, topBackRight, bottomFrontLeft, bottomFrontRight, bottomBackleft, bottomBackRight };
        }
    }
}
