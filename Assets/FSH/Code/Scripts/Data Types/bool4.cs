using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fishy.DataTypes {
    public struct bool4
    {
        public bool x, y, z, w;

        public static bool4 True
        {
            get {
                return new bool4(true, true, true, true);
            }
        }
        public static bool4 False
        {
            get {
                return new bool4(false, false, false, false);
            }
        }

        public override string ToString()
        {
            char[] chars = new char[4];

            for (int i = 0; i < 4; i++) {
                if (this[i])
                    chars[i] = '1';
                else
                    chars[i] = '0';
            }
            return new string(chars);
        }

        public bool4(bool x, bool y, bool z, bool w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public bool4(bool[] bools)
        {
            this.x = bools[0];
            this.y = bools[1];
            this.z = bools[2];
            this.w = bools[3];
        }
        public bool4(bool4 bools)
        {
            this.x = bools.x;
            this.y = bools.y;
            this.z = bools.z;
            this.w = bools.w;
        }

        public bool this[int index]
        {
            get {
                switch (index) {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    case 3:
                        return w;
                    default:
                        throw new System.IndexOutOfRangeException("bool4 index out of range");

                }

            }
            set {

                switch (index) {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new System.IndexOutOfRangeException("bool4 index out of range");
                }
                return;

            }
        }

        public static bool operator ==(bool4 x, bool4 y)
        {
            return x.x == y.x && x.y == y.y && x.z == y.z && x.w == y.w;
        }

        public static bool operator !=(bool4 x, bool4 y)
        {
            return x.x != y.x || x.y != y.y || x.z != y.z || x.w != y.w;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is bool4 && this == (bool4)obj;
        }

    }
}
