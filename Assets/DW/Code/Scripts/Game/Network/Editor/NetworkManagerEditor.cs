using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DW.Network {
    [CustomEditor(typeof(NetworkManager))]
	public class NetworkManagerEditor : Editor {
        #region Variables
        //Public & Serialized
        NetworkManager manager;
        //Private

        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        private void OnEnable()
        {
            manager = (NetworkManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            string debugInfo = "";
            if (manager.Network != null) {
                if (manager.Network.GetType() == typeof(ServerInstance)) {
                    debugInfo += GetServerInfo((ServerInstance)manager.Network);
                } else if (manager.Network.GetType() == typeof(ClientInstance)) {
                    debugInfo += GetClientInfo((ClientInstance)manager.Network);
                }
            }

            GUILayout.Space(20);
            EditorGUILayout.TextArea(debugInfo, GUILayout.Height(100));
            GUILayout.Space(20);
        }
        #endregion;

        #region Custom Methods
        private string GetServerInfo(ServerInstance instance)
        {
            string info = "";
            info += "Server Status: " + instance.Server.Status.ToString() + "\n";
            info += "NUID: " + instance.Identifier + "\n";
            foreach (var connection in instance.Server.Connections) {
                info += "Conection: " + connection.RemoteUniqueIdentifier + "\n";
            }
            info += "\n";

            return info;
        }

        private string GetClientInfo(ClientInstance instance)
        {
            string info = "";
            info += "Client Status: " + instance.Client.ConnectionStatus.ToString() + "\n";
            info += "Local ID: " + instance.Identifier + "\n";
            if (instance.Client.ServerConnection != null) {
                info += "Server ID: " + instance.Client.ServerConnection.RemoteUniqueIdentifier + "\n";
            } else {
                info += "No Server conection!" + "\n";
            }
            
            info += "\n";

            return info;
        }
        #endregion
    }
}
