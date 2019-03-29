using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fishy.Meshes;

namespace Fishy.Grids {
	public static class GridDrawer {
		#region Properties

		#endregion;

		#region Unity Methods

		#endregion;

        #region Custom Methods
        public static void GenerateGridLines<T>(Grid3D<T> grid, Vector3 worldAnchor, float scale, Material material)
        {
            MeshBuilder builder = new MeshBuilder();
            float size = 0.02f;
            float halfScale = scale;

            //Y
            for (int x = grid.Volume.u.x; x < grid.Volume.v.x; x++) {
                for (int z = grid.Volume.u.z; z < grid.Volume.v.z; z++) {
                    BuildLine(GridToWorld(new Vector3(x, 0, z), worldAnchor, scale), new Vector3(size, halfScale * 0.5f * grid.Volume.Height, size), material);
                }
            }

            //z
            for (int x = grid.Volume.u.x; x < grid.Volume.v.x; x++) {
                for (int y = grid.Volume.u.y; y < grid.Volume.v.y; y++) {
                    BuildLine(GridToWorld(new Vector3(x, y, 0), worldAnchor, scale), new Vector3(size, size, halfScale * grid.Volume.Depth), material);
                }
            }

            //X
            for (int z = grid.Volume.u.z; z < grid.Volume.v.z; z++) {
                for (int y = grid.Volume.u.y; y < grid.Volume.v.y; y++) {
                    BuildLine(GridToWorld(new Vector3(0, y, z), worldAnchor, scale), new Vector3(halfScale * grid.Volume.Width, size, size), material);
                }
            }



        }
        
        private static void BuildLine(Vector3 position, Vector3 scale, Material material)
        {
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            line.transform.position = position;
            line.transform.localScale = scale;

            line.GetComponent<Renderer>().material = material;
        }

        private static Vector3 GridToWorld(Vector3 gridPos, Vector3 worldAnchor, float scale)
        {
            return worldAnchor + (gridPos * scale);
        }
        #endregion
	}
}
