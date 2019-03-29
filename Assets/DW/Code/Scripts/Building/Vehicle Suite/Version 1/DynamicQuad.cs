using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fishy.Meshes;

namespace DW.Building.DepreciatedShipSuite {
    [System.Serializable]
    public class DynamicQuad
    {
        public DynamicVertex a;
        public DynamicVertex b;
        public DynamicVertex c;
        public DynamicVertex d;
        public Vector3 normal;

        public DynamicQuad(DynamicVertex a, DynamicVertex b, DynamicVertex c, DynamicVertex d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;

            Vector3 width = d.point - a.point;
            Vector3 length = b.point - a.point;
            normal = Vector3.Cross(length, width).normalized;
        }
        public void BuildQuad(MeshBuilder meshBuilder)
        {
            meshBuilder.Vertices.Add(a.point);
            meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
            meshBuilder.Normals.Add(normal);

            meshBuilder.Vertices.Add(b.point);
            meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
            meshBuilder.Normals.Add(normal);

            meshBuilder.Vertices.Add(c.point);
            meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
            meshBuilder.Normals.Add(normal);

            meshBuilder.Vertices.Add(d.point);
            meshBuilder.UVs.Add(new Vector2(1.0f, 0.0f));
            meshBuilder.Normals.Add(normal);

            int baseIndex = meshBuilder.Vertices.Count - 4;

            meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
            meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
        }
    }
}
