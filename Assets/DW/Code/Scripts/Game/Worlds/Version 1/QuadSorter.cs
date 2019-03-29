using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW.Worlds.V1 {
    public class QuadSorter : IComparer<Quad>
    {
        public int Compare(Quad x, Quad y)
        {
            if (x.level > y.level)
                return 1;
            if (x.distance > y.distance && x.level == y.level)
                return 1;

            return -1;
        }
    }
}
