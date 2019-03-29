using UnityEngine;

namespace Fishy.DataTypes {
    [System.Serializable]
	public struct Volume
    {
        public Short3 u;
        public Short3 v;
        public int Width { get { return (1 + Mathf.Abs(v.x - u.x)); } }
        public int Height { get { return (1 + Mathf.Abs(v.y - u.y)); } }
        public int Depth { get { return (1 + Mathf.Abs(v.z - u.z)); } }
        public int Area { get { return Width * Height * Depth; } }

        public Vector3 Center {
            get {
                return (u != v) ? new Vector3((u.x + v.x) / 2f, (u.y + v.y) / 2f, (u.z + v.z) / 2f) : (Vector3)u;
            }
        }

        public void SetUV(Short3 a, Short3 b)
        {
            if (a == b) {
                u = a;
                v = b;
                return;
            }
            u = new Short3((short)Mathf.Min(a.x, b.x), (short)Mathf.Min(a.y, b.y), (short)Mathf.Min(a.z, b.z));
            v = new Short3((short)Mathf.Max(a.x, b.x), (short)Mathf.Max(a.y, b.y), (short)Mathf.Max(a.z, b.z));
        }

        public void Santize()
        {
            SetUV(u, v);
        }

        public Volume(Short3 a, Short3 b, bool isOrdered = false)
        {
            u = a;
            v = b;

            if (!isOrdered) {
                SetUV(a, b);
            }
        }

        public Volume(Short3 point)
        {
            this.u = point;
            this.v = point;
        }

        public override string ToString()
        {
            return (u != v) ? (u.ToString() + " -> " + v.ToString()): u.ToString();
        }

        public static Volume zero { get { return new Volume(Short3.zero); } }

        /// <summary>
        /// If the passed point falls within (or on) the volume, returns true
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Short3 point)
        {
            return (
                point.x >= u.x && point.x <= v.x &&
                point.y >= u.y && point.y <= v.y &&
                point.z >= u.z && point.z <= v.z
            );
        }

        /// <summary>
        /// Returns true if the passed volume is within or equal to our own
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public bool Contains(Volume volume)
        {
            return (
                volume.u.x >= u.x && volume.u.y >= u.y && volume.u.z >= u.z &&
                volume.v.x <= v.x && volume.v.y <= v.y && volume.v.z <= v.z
            );
        }

        /// <summary>
        /// Returns true if collision occurs between the two volumes
        /// </summary>
        /// <param name="volume">volume to check against</param>
        /// <returns></returns>
        public bool Intersects(Volume volume)
        {
            return (
                this.v.x >= volume.u.x && u.x <= volume.v.x &&
                this.v.y >= volume.u.y && u.y <= volume.v.y &&
                this.v.z >= volume.u.z && u.z <= volume.v.z
            );
        }

        /// <summary>
        /// Returns a new volume containing the overlap between two volumes
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public Volume IntersectionVolume(Volume volume)
        {
            Short3 min = new Short3((short)Mathf.Max(u.x, volume.u.x), (short)Mathf.Max(u.y, volume.u.y), (short)Mathf.Max(u.z, volume.u.z));
            Short3 max = new Short3((short)Mathf.Min(v.x, volume.v.x), (short)Mathf.Min(v.y, volume.v.y), (short)Mathf.Min(v.z, volume.v.z));

            return new Volume(min, max);
        }

        public static bool operator ==(Volume a, Volume b)
        {
            return (a.u == b.u && a.v == b.v);
        }

        public static bool operator !=(Volume a, Volume b)
        {
            return (a.u != b.u || a.v != b.v);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
