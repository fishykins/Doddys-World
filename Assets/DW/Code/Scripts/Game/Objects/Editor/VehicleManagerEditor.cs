using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DW.Objects {
    [CustomEditor(typeof(ObjectManager))]
    public class VehicleManagerEditor : Editor
    {
        ObjectManager manager;

        private void OnEnable()
        {
            manager = (ObjectManager)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }

        
    }
}
