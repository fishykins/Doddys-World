using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DW.Building.DepreciatedShipSuite {

    [CustomEditor(typeof(ShipManager))]
    public class ShipManagerEditor : Editor
    {
        ShipManager obj;

        private void OnEnable()
        {
            obj = (ShipManager)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Save")) {
                obj.SaveBuild(obj.fileName);
            }

            if (GUILayout.Button("Load")) {
                obj.LoadBuild(obj.fileName);
            }
        }
    }
}
