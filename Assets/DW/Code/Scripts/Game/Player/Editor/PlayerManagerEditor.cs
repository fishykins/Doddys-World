using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DW.Player {
    [CustomEditor(typeof(PlayerManager))]
    public class PlayerManagerEditor : Editor
    {
        #region Variables
        //Public & Serialized
        PlayerManager manager;
        //Private

        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        private void OnEnable()
        {
            manager = (PlayerManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            string debugInfo = "";
            debugInfo += "Status: " + manager.Status + "\n";

            if (manager.ControlBody != null) {
                debugInfo += "Control Target: " + manager.ControlBody.Transform.name;
            }

            GUILayout.Space(20);
            EditorGUILayout.TextArea(debugInfo, GUILayout.Height(50));
            GUILayout.Space(20);
        }
        #endregion;
    }
}
