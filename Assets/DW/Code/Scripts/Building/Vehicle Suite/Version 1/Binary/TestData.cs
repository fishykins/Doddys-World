using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unifish.Serialization;

namespace DW.Building.DepreciatedShipSuite {
    [Serializable]
	public class TestData
    {
        public string name;
        public float size;
        public int verts;
        public Vector3 position;

        public TestData(string name, float size, int verts, Vector3 position)
        {
            this.name = name;
            this.size = size;
            this.verts = verts;
            this.position = position;
        }

        public void DebugInfo()
        {
            Debug.Log(name + ", " + size + ", " + verts);
        }
    }
}
