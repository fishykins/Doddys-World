using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DW.Vehicles {
    [CustomEditor(typeof(VehicleController))]
    public class VehicleControllerEditor : Editor
    {

        private VehicleController vehicle;

        public void OnEnable()
        {
            vehicle = (VehicleController)target;
        }

        public override void OnInspectorGUI()
        {
            string info = "Origin: " + vehicle.Origin + ", Host: " + vehicle.Host + ", Prefab: " + vehicle.Prefab + ", UID: " + vehicle.Index;
            if (vehicle.physicsBodies != null)
                info += ", PhysicsBody count: " + vehicle.physicsBodies.Count;

            EditorGUILayout.LabelField(info, EditorStyles.label);

            //if (GUILayout.Button("Build Object"))
                //vehicle.BuildObject();

            DrawDefaultInspector();
        }
    }
}
