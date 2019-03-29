using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DW.Worlds
{
    [CustomEditor(typeof(WorldManager))]
    public class VehicleManagerEditor : Editor
    {
        WorldManager manager;

        private void OnEnable()
        {
            manager = (WorldManager)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            string debugInfo = "";
            debugInfo += "Status: " + manager.Status;

            GUILayout.Space(20);
            EditorGUILayout.TextArea(debugInfo, GUILayout.Height(50));
            GUILayout.Space(20);

            if (GUILayout.Button("Spawn Test Objects")) {
                manager.SpawnTestObjects();
            }
        }


    }
}
