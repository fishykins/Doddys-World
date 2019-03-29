using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DW.Worlds
{
    [CustomEditor(typeof(SceneInstance))]
    public class SceneInstanceEditor : Editor
    {
        SceneInstance scene;

        private void OnEnable()
        {
            scene = (SceneInstance)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            string debugInfo = "";
            debugInfo += "Status: " + scene.Status + "\n";
            debugInfo += "World Count: " + scene.Worlds.Count + "\n";
            debugInfo += "G-Object Count: " + scene.GravityBodies.Count + "\n";
            debugInfo += "Vehicle Count: " + scene.Vehicles.Count + "\n";

            GUILayout.Space(20);
            EditorGUILayout.TextArea(debugInfo, GUILayout.Height(100));
            GUILayout.Space(20);
        }


    }
}
