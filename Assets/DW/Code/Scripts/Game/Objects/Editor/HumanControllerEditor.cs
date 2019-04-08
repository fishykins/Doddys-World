using UnityEngine;
using UnityEditor;

namespace DW.Objects
{
    [CustomEditor(typeof(HumanNetController))]
    public class HumanControllerEditor : Editor
    {

        private HumanNetController controller;

        public void OnEnable()
        {
            controller = (HumanNetController)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Origin: " + controller.Origin, EditorStyles.label);
            EditorGUILayout.LabelField("Index: " + controller.Index, EditorStyles.label);
            EditorGUILayout.LabelField("Prefab: " + controller.Prefab, EditorStyles.label);
            EditorGUILayout.LabelField("Host: " + controller.Host, EditorStyles.largeLabel);

            base.OnInspectorGUI();

        }
    }
}