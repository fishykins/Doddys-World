using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DW.Worlds.V1 {
    [CustomEditor(typeof(World))]
    public class WorldEditor : Editor
    {

        public enum Tab
        {
            General, Generation, Visual, Debug
        }
        Tab tab = Tab.General;

        private World world;
        SerializedProperty material;

        string[] tabNames = { "General", "Generation", "Visual", "Debug" };

        public void OnEnable()
        {
            world = (World)target;
            material = serializedObject.FindProperty("material");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Quads: " + world.Quads.Count, EditorStyles.label);

            //EditorGUILayout.Space();
            tab = (Tab)GUILayout.Toolbar((int)tab, tabNames, EditorStyles.toolbarButton);
            EditorGUILayout.Space();

            switch (tab) {
                case Tab.General:
                    world.radius = EditorGUILayout.FloatField("Radius", world.radius);
                    break;
                case Tab.Generation:
                    world.computeShader = (ComputeShader)EditorGUILayout.ObjectField("Compute Shader", world.computeShader, typeof(ComputeShader), false);
                    world.biomeOffset = EditorGUILayout.Vector3Field("Biome offset", world.biomeOffset);
                    //world.heightCalculation = (HeightCalculation)EditorGUILayout.EnumPopup(new GUIContent("Height Measurment Mode", "Is height measured in real world units (absolute), or as a ratio of radius (relative)?"), world.heightCalculation);
                    break;
                case Tab.Visual:
                    EditorGUILayout.PropertyField(material);
                    GUILayout.Space(5f);
                    //EditorGUILayout.LabelField("Terrain lerp Height: " + world.heightRange, EditorStyles.label);
                    //EditorGUILayout.MinMaxSlider(ref world.heightRange.x, ref world.heightRange.y, 0f, 1f);
                    break;
                case Tab.Debug:
                    DrawDefaultInspector();
                    break;
                default:
                    break;
            }


            GUILayout.Space(20);
            EditorGUILayout.TextArea(world.inspectorLog, GUILayout.Height(100));
            GUILayout.Space(20);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
