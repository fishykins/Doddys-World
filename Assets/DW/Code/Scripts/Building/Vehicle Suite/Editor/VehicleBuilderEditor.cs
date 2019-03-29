using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DW.Building.VehicleSuite4 {
    [CustomEditor(typeof(VehicleManager))]
    public class VehicleBuilderEditor : Editor
    {
        public enum VehicleBuilderTab
        {
            General, Visual, Input, Mesh, Debug
        }

        string[] tabNames = { "General", "Visual", "Input", "Mesh", "Debug"};

        VehicleManager builder;
        VehicleBuilderTab tab = VehicleBuilderTab.General;
        SerializedProperty mainMaterial;
        SerializedProperty ghostMaterial;
        SerializedProperty selectedMaterial;
        SerializedProperty pointerPrefab;

        public void OnEnable()
        {
            builder = (VehicleManager)target;

            mainMaterial = serializedObject.FindProperty("mainMaterial");
            ghostMaterial = serializedObject.FindProperty("ghostMaterial");
            selectedMaterial = serializedObject.FindProperty("selectedMaterial");
            pointerPrefab = serializedObject.FindProperty("pointerPrefab");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            tab = (VehicleBuilderTab)GUILayout.Toolbar((int)tab, tabNames, EditorStyles.toolbarButton);
            EditorGUILayout.Space();

            switch (tab) {
                case VehicleBuilderTab.General:
                    EditorGUILayout.LabelField("Grid Options", EditorStyles.boldLabel);
                    builder.gridSize = (short)EditorGUILayout.IntSlider("Size", (int)builder.gridSize, 4, 64);
                    builder.gridScale = EditorGUILayout.Slider("Scale", builder.gridScale, 0.1f, 4f);
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Generation", EditorStyles.boldLabel);
                    builder.generationType = (GenerationType)EditorGUILayout.EnumPopup(new GUIContent("Generation Mode", "Blank bp design"), builder.generationType);
                    switch (builder.generationType) {
                        case GenerationType.sphere:
                        case GenerationType.cube:
                            builder.generatorRadius = EditorGUILayout.IntSlider("Radius", builder.generatorRadius, 4, Mathf.Min(builder.gridSize, 32));
                            builder.randomGeneratorVolume = EditorGUILayout.Slider("Volume Fill", builder.randomGeneratorVolume, 0f, 1f);
                            builder.generationThickness = EditorGUILayout.Slider("Thickness", builder.generationThickness, 0.1f, 1f);
                            break;
                        case GenerationType.single:
                            builder.defaultCubeSize = EditorGUILayout.IntSlider("Starting cube size", builder.defaultCubeSize, 1, 9);
                            break;
                        default:
                            break;
                    }
                    EditorGUILayout.Space();
                    break;
                case VehicleBuilderTab.Visual:
                    EditorGUILayout.PropertyField(mainMaterial);
                    EditorGUILayout.PropertyField(ghostMaterial);
                    EditorGUILayout.PropertyField(selectedMaterial);
                    EditorGUILayout.PropertyField(pointerPrefab);
                    break;
                case VehicleBuilderTab.Input:
                    EditorGUILayout.LabelField("Control Settings", EditorStyles.boldLabel);
                    builder.cameraRange = EditorGUILayout.Slider("Camera Range", builder.cameraRange, 5f, 100f);
                    builder.cameraDistance = EditorGUILayout.Slider("Camera targeting distance", builder.cameraDistance, 5f, builder.cameraRange);
                    builder.raycastMultiplier = EditorGUILayout.Slider("Raycast multiplier", builder.raycastMultiplier, 1f, 10f);
                    builder.editMode = (EditMode)EditorGUILayout.EnumPopup(new GUIContent("Edit Mode", "Edit mode to use"), builder.editMode);
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Input", EditorStyles.boldLabel);
                    builder.keyDragSelect = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Select on Three Axis", "Holding this key will allow for drag placement"), builder.keyDragSelect);
                    builder.keySingleAxis = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Select on a single Axis", "Goon"), builder.keySingleAxis);
                    builder.keyBake = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Bake key", "Button to press when you want to bake the mesh"), builder.keyBake);
                    builder.keyCycleMode = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Cycle Mode", "Button to press when you want to cycle through build modes"), builder.keyCycleMode);
                    builder.scrollWheelSensitivity = EditorGUILayout.Slider("Scrollwheel Sensitivity", builder.scrollWheelSensitivity, 0.1f, 20f);
                    builder.singleClickTime = EditorGUILayout.Slider("Click Max Duration", builder.singleClickTime, 0.01f, 0.5f);

                    break;
                case VehicleBuilderTab.Mesh:
                    EditorGUILayout.LabelField("Mesh Optimization (" + builder.meshCompressionRate + "%)", EditorStyles.boldLabel);
                    string optString = builder.OptimizationIndex + "/" + builder.OptimizationCount;
                    EditorGUILayout.LabelField("Optimiaztion loop: ", optString);
                    builder.shapesPerFrame = EditorGUILayout.IntSlider("Shapes per frame", builder.shapesPerFrame, 0, 16);
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Mesh Construction", EditorStyles.boldLabel);
                    builder.vertsPerMesh = EditorGUILayout.IntSlider("Max Verticies per mesh", builder.vertsPerMesh, 8000, 60000);
                    builder.maxSelectedShaped = EditorGUILayout.IntSlider("Max number of selected shapes", builder.maxSelectedShaped, 16, 128);
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Serialization", EditorStyles.boldLabel);
                    builder.directoryName = EditorGUILayout.TextField("Save/Load Folder", builder.directoryName);
                    builder.fileName = EditorGUILayout.TextField("File Name", builder.fileName);
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Save Grid")) {
                        
                    }
                    if (GUILayout.Button("Load Grid")) {

                    }
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Export Mesh")) {

                    }

                    this.Repaint();
                    break;
                case VehicleBuilderTab.Debug:
                    EditorGUILayout.TextArea(builder.debugInfo, GUILayout.Height(100));
                    EditorGUILayout.Space();
                    DrawDefaultInspector();
                    break;
                default:
                    DrawDefaultInspector();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
