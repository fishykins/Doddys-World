using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fishy.DataTypes;

namespace DW.Building.VehicleSuite4 {
	public interface IShape  {
        //We dont need to store global position, as that is held in the grid. We only need local vertex data

        Quad[] GetQuads(Volume gridVolume, Vector3 gridWorldAnchor, float gridScale);

        //Short3[] GetLocalVertecies();
        //Vector3[] GetWorldVertecies(Volume gridArea, Vector3 worldAnchor, float worldScale);
        /// <summary>
        /// Deforms a shape, according to its particular rules
        /// </summary>
        /// <param name="localStart">The inital local position to deform from</param>
        /// <param name="localEnd">The local position to deform to</param>
        void Deform(Vector3 localStart, Vector3 localEnd);
	}
}
