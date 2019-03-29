using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fishy.DataTypes {
    public struct int2
    {
        public int x;
        public int y;

        public int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int this[int index]
        {
            get {
                if (index == 0)
                    return x;

                if (index > 1)
                    throw new System.IndexOutOfRangeException("int2 index out of range");

                return y;
            }
            set {

                if (index == 0) {
                    x = value;
                    return;
                }
                if (index > 1)
                    throw new System.IndexOutOfRangeException("int2 index out of range");

                y = value;

            }
        }
    }
}
