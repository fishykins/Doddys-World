using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DW.Vehicles {
    [CustomEditor(typeof(VehicleManager))]
    public class VehicleManagerEditor : Editor
    {
        VehicleManager manager;

        private void OnEnable()
        {
            manager = (VehicleManager)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }

        
    }
}
