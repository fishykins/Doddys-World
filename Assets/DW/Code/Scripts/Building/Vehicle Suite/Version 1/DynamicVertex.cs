using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW.Building.DepreciatedShipSuite {
    [System.Serializable]
	public class DynamicVertex
    {
        public Vector3 point = new Vector3(307,307,307);
        public bool isActive = true;

        public bool IsrealVertex()
        {
            return (point - new Vector3(307, 307, 307)).magnitude > 0.01f;
        }
    }
}
