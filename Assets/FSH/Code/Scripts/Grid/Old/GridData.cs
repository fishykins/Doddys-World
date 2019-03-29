using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fishy.GridsOld {
    /// <summary>
    /// This is a snapshot of a collection of grid points, providing extreme values and an average
    /// </summary>
    public struct GridData
    {
        public Vector3 min;
        public Vector3 max;
        public Vector3 average;

        public GridData(Vector3 min, Vector3 max, Vector3 average)
        {
            this.min = min;
            this.max = max;
            this.average = average;
        }

        public GridData(List<Vector3> gridPoints)
        {
            int minX = int.MaxValue; int maxX = int.MinValue;
            int minY = int.MaxValue; int maxY = int.MinValue;
            int minZ = int.MaxValue; int maxZ = int.MinValue;

            foreach (Vector3 v in gridPoints) {
                if (v.x < minX) minX = (int)v.x; if (v.x > maxX) maxX = (int)v.x;
                if (v.y < minY) minY = (int)v.y; if (v.y > maxY) maxY = (int)v.y;
                if (v.z < minZ) minZ = (int)v.z; if (v.z > maxZ) maxZ = (int)v.z;
            }

            min = new Vector3(minX, minY, minZ);
            max = new Vector3(maxX, maxY, maxZ);
            average = (min + max) / 2f;
        }
    }
}
