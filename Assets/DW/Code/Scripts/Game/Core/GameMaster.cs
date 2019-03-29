using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW {
    /// <summary>
    /// This Script is responsible for starting the entire game. It will create NetowrkManagers for every instance (client, server etc) and set them off
    /// doing their respective tasks. This is the only module that is "aware" of both client and server, so any cross instance work should be done here.
    /// </summary>
	public class GameMaster : MonoBehaviour {
        #region Variables
        //Public
        public static GameMaster instance; 

        [Header("Network Settings")]
        [SerializeField]
        private string lidgrenID = "DoddysWorld";
        [SerializeField]
        private int tickRate = 64;
        [SerializeField,Range(0,100),Tooltip("Only shows console messages with priority higher or equal to this")]
        private int infoThreshold = 0;

        //Private
        private List<ApplicationRole> applicationRoles;
        private Dictionary<string, SceneInstance> sceneInstances = new Dictionary<string, SceneInstance>();
        #endregion;

        #region Properties
        public List<ApplicationRole> ApplicationRoles { get { return applicationRoles; } }
        public Dictionary<string, SceneInstance> SceneInstances { get { return sceneInstances; } }
        public string LidgrenID { get { return lidgrenID; } }
        public int LogThreshhold { get { return infoThreshold; } }
        public float TickRate { get { return tickRate; } }
        #endregion;

        #region Unity Methods
        void Awake () {
			if (!instance) {
                instance = this;
            } else {
                Debug.LogWarning("There are more than one 'GameLibrary' in this scene- libraries are single instance only!");
            }
		}

        private void Start()
        {
            InitializeGame();
        }
        #endregion;

        #region Custom Methods
        private void InitializeGame()
        {
            InitializeGameRole();
            GenerateSceneInstances();
        }

        private void InitializeGameRole()
        {
            applicationRoles = new List<ApplicationRole>();

#if SERVER
            applicationRoles.Add(ApplicationRole.host);
#endif

#if CLIENT
            applicationRoles.Add(ApplicationRole.client);
#endif

#if LOCAL
            applicationRoles.Add(ApplicationRole.local);
#endif
        }

        private void GenerateSceneInstances()
        {
            if (applicationRoles.Contains(ApplicationRole.host)) {
                GenerateInstance("Server", "S_", ApplicationRole.host, "red", "Server", true);
            }
            if (applicationRoles.Contains(ApplicationRole.client)) {
                GenerateInstance("Client", "C_", ApplicationRole.client, "blue", "Client");
            }
            if (applicationRoles.Contains(ApplicationRole.local)) {
                GenerateInstance("Local", "L_", ApplicationRole.local, "orange", "Client");
            }
        }

        private void GenerateInstance(string name, string preffix, ApplicationRole role, string colour, string layer, bool headless = false)
        {
            GameObject sceneObject = new GameObject(name);
            SceneInstance scene = sceneObject.AddComponent<SceneInstance>();
            scene.Initialize(preffix, role, colour, layer, headless);
            sceneInstances.Add(name, scene);
        }

        public SceneInstance GetSceneInstance(string name)
        {
            SceneInstance _inst;
            return (sceneInstances.TryGetValue(name, out _inst)) ? _inst : null;
        }
        #endregion
    }
}
