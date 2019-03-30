using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unifish.Meshes;
using Unifish.DataTypes;
using Unifish.Grids;

namespace DW.Building.VehicleSuite3
{
    /// <summary>
    /// The default shape carries no data, and will allways create a cube. Useful for when a shape has not been changed
    /// </summary>
    [System.Serializable]
    public class DefautShape : IShape {

        private byte edgeID = 63; //63 is the value for all faces being vissible

        /// <summary>
        /// Passes a standard cube to the MeshBuilder
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="worldPosition"></param>
        /// <param name="scale"></param>
        public void PassThroughMeshBuilder(MeshBuilder builder, Vector3 worldPosition, Vector3 scale)
        {
            float px = 0.5f * scale.x;
            float py = 0.5f * scale.y;
            float pz = 0.5f * scale.z;
            Vector3 topFrontLeft = worldPosition + new Vector3(-px, py, pz);
            Vector3 topFrontRight = worldPosition + new Vector3(px, py, pz);
            Vector3 topBackLeft = worldPosition + new Vector3(-px, py, -pz);
            Vector3 topBackRight = worldPosition + new Vector3(px, py, -pz);
            Vector3 bottomFrontLeft = worldPosition + new Vector3(-px, -py, pz);
            Vector3 bottomFrontRight = worldPosition + new Vector3(px, -py, pz);
            Vector3 bottomBackleft = worldPosition + new Vector3(-px, -py, -pz);
            Vector3 bottomBackRight = worldPosition + new Vector3(px, -py, -pz);

            bool[] quads = Unifish.ByteConverter.ByteToBoolArray(edgeID, 6);
            if (quads[0]) BuildQuad(builder, topFrontRight, topBackRight, topBackLeft, topFrontLeft); //Top
            if (quads[1]) BuildQuad(builder, bottomBackleft, bottomBackRight, bottomFrontRight, bottomFrontLeft); //Bottom
            if (quads[2]) BuildQuad(builder, bottomBackleft, bottomFrontLeft, topFrontLeft, topBackLeft); //Left
            if (quads[3]) BuildQuad(builder, topFrontRight, bottomFrontRight, bottomBackRight, topBackRight); //Right
            if (quads[4]) BuildQuad(builder, topFrontRight, topFrontLeft, bottomFrontLeft, bottomFrontRight); //Front
            if (quads[5]) BuildQuad(builder, bottomBackleft, topBackLeft, topBackRight, bottomBackRight); //back
            
        }

        public void BuildQuad(MeshBuilder meshBuilder, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 width = d - a;
            Vector3 length = b - a;
            Vector3 normal = Vector3.Cross(length, width).normalized;

            float uvScale = width.magnitude;

            meshBuilder.Vertices.Add(a);
            meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
            meshBuilder.Normals.Add(normal);

            meshBuilder.Vertices.Add(b);
            meshBuilder.UVs.Add(new Vector2(0.0f, uvScale));
            meshBuilder.Normals.Add(normal);

            meshBuilder.Vertices.Add(c);
            meshBuilder.UVs.Add(new Vector2(uvScale, uvScale));
            meshBuilder.Normals.Add(normal);

            meshBuilder.Vertices.Add(d);
            meshBuilder.UVs.Add(new Vector2(uvScale, 0.0f));
            meshBuilder.Normals.Add(normal);

            int baseIndex = meshBuilder.Vertices.Count - 4;

            meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
            meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
        }

        public void CalculateNeighbors(Grid3D<IShape> grid, bool forceNeighborUpdate = false)
        {
            edgeID = 63;
            Volume volume = grid.GetVolume(this);

            Volume[] edges = grid.GetAdjoiningVolumes(volume).ToArray();

            for (int i = 0; i < 6; i++) {
                if (EdgeCheck(grid, edges[i], forceNeighborUpdate)) edgeID -= (byte)Mathf.Pow(2, i);
            }
        }

        public Vector3 GetClosestVertex(Vector3 worldPosition, Vector3 shapeWorldPosition, Vector3 scale)
        {
            float px = 0.5f * scale.x;
            float py = 0.5f * scale.y;
            float pz = 0.5f * scale.z;

            worldPosition = (worldPosition - shapeWorldPosition);

            float x = Mathf.Clamp(worldPosition.x, -px, px);
            float y = Mathf.Clamp(worldPosition.y, -py, py);
            float z = Mathf.Clamp(worldPosition.z, -pz, pz);
            return new Vector3(x, y, z);
        }


        private bool EdgeCheck(Grid3D<IShape> grid, Volume edge, bool forceNeighborUpdate)
        {
            List<IShape> shapes = grid.GetObjects(edge);

            if (shapes.Count == 0) return false;

            if (shapes.Count == 1 && edge.Area == 1) {
                if (forceNeighborUpdate) shapes[0].CalculateNeighbors(grid, false);
                return true;
            }

            //calculate the volume each edge object takes up and if we get a volume of 1 obscure edge
            int area = 0;

            foreach (IShape shape in shapes) {
                Volume volume = grid.GetVolume(shape);
                area += edge.IntersectionVolume(volume).Area;
                if (forceNeighborUpdate) shape.CalculateNeighbors(grid, false);
            }

            if (area >= edge.Area) return true;

            return false;
        }
    }
}
