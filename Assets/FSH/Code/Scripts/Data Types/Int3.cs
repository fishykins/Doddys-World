using System;
using UnityEngine;

namespace Fishy.DataTypes {
    [System.Serializable]
    public struct Int3 
    {
        /// <summary>
        /// x component
        /// </summary>
        public int x;

        /// <summary>
        /// y component
        /// </summary>
        public int y;

        /// <summary>
        /// z component
        /// </summary>
        public int z;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Int3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Returns a string representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}]", x, y, z);
        }

        /// <summary>
        /// Automatic conversion from Int3 to Vector3
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator Vector3(Int3 rValue)
        {
            return new Vector3(rValue.x, rValue.y, rValue.z);
        }

        /// <summary>
        /// Automatic conversion from Vector3 to Int3
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator Int3(Vector3 rValue)
        {
            return new Int3(Mathf.RoundToInt(rValue.x), Mathf.RoundToInt(rValue.y), Mathf.RoundToInt(rValue.z));
        }
    }
}
