using UnityEngine;
using UnityEditor;

namespace DW.Vehicles
{
    [CustomEditor(typeof(HumanController))]
    public class HumanControllerEditor : Editor
    {

        private HumanController controller;

        public void OnEnable()
        {
            controller = (HumanController)target;
        }

        public override void OnInspectorGUI()
        {
            string info = "Origin: " + controller.Origin + ", Host: " + controller.Host + ", Prefab: " + controller.Prefab;
            EditorGUILayout.LabelField(info, EditorStyles.label);

            if (controller.PhysicsBodies != null)
                EditorGUILayout.LabelField("PhysicsBody count: " + controller.PhysicsBodies.Count, EditorStyles.label);


            base.OnInspectorGUI();

        }
    }
}