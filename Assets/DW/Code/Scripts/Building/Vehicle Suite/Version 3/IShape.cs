using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unifish.Meshes;
using Unifish.Grids;

namespace DW.Building.VehicleSuite3
{
	public interface IShape 
    {
        /// <summary>
        /// Adds the shape to meshBuilder, based on passed position and scale. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="worldPosition"></param>
        /// <param name="scale"></param>
        void PassThroughMeshBuilder(MeshBuilder builder, Vector3 worldPosition, Vector3 scale);

        void CalculateNeighbors(Grid3D<IShape> grid, bool forceNeighborUpdate = false);

        Vector3 GetClosestVertex(Vector3 worldPosition, Vector3 shapeWorldPosition, Vector3 scale);
    }
}
