using System;
using UnityEngine;

namespace Fishy.DataTypes
{
    [System.Serializable]
    public struct Short3
    {
        /// <summary>
        /// x component
        /// </summary>
        public short x;

        /// <summary>
        /// y component
        /// </summary>
        public short y;

        /// <summary>
        /// z component
        /// </summary>
        public short z;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Short3(short x, short y, short z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Short3 zero { get { return new Short3(0, 0, 0); } }
        public static Short3 one { get { return new Short3(1, 1, 1); } }

        public float magnitude { get { return ((Vector3)this).magnitude; } }

        /// <summary>
        /// Returns a string representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}]", x, y, z);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Automatic conversion from Short3 to Vector3
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator Vector3(Short3 rValue)
        {
            return new Vector3(rValue.x, rValue.y, rValue.z);
        }

        /// <summary>
        /// Automatic conversion from Short3 to Volume
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Volume(Short3 value)
        {
            return new Volume(value, value);
        }

        public static bool operator ==(Short3 a, Short3 b)
        {
            return (a.x == b.x && a.y == b.y && a.z == b.z);
        }

        public static Short3 operator -(Short3 a)
        {
            return new Short3((short)-a.x, (short)-a.y, (short)-a.z);
        }

        public static Short3 operator -(Short3 a, Short3 b)
        {
            return new Short3((short)(a.x - b.x), (short)(a.y - b.y), (short)(a.z - b.z));
        }

        public static Short3 operator +(Short3 a, Short3 b)
        {
            return new Short3((short)(a.x + b.x), (short)(a.y + b.y), (short)(a.z + b.z));
        }

        public static bool operator !=(Short3 a, Short3 b)
        {
            return (a.x != b.x || a.y != b.y || a.z != b.z);
        }

        /// <summary>
        /// Automatic conversion from Vector3 to Int3
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator Short3(Vector3 rValue)
        {
            return new Short3((short)Mathf.RoundToInt(rValue.x), (short)Mathf.RoundToInt(rValue.y), (short)Mathf.RoundToInt(rValue.z));
        }

        public static Short3 operator *(Short3 a, float b)
        {
            return new Vector3(a.x * b, a.y * b, a.z * b);
        }

        
    }
}
