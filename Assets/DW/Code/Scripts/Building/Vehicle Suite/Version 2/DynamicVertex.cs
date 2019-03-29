using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW.Building.LegacyBuildingSuite
{
    [System.Serializable]
    public struct DynamicVertex
    {
        public Vector3 point;
        public bool isActive;

        public DynamicVertex(Vector3 point, bool isActive = true)
        {
            this.point = point;
            this.isActive = isActive;
        }
    }
}
