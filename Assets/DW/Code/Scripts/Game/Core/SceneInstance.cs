using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Network;
using DW.Worlds;
using DW.Vehicles;
using DW.Physics;
using DW.Player;
using Unifish;
using PlanetaryTerrain;


namespace DW {
    /// <summary>
    /// A sceneInstance is the main, most important thing for each game "instance". All objects should have access to one of these!
    /// When something needs doing, it should be done here. The SceneInstance will deligate to one of its managers to do the actual work
    /// </summary>
	public class SceneInstance : MonoBehaviour {
        #region Variables
        //Public & Serialized

        //Private
        private string preffix;
        private ApplicationRole role;
        private LayerMask layer;
        private bool headless; //If this instance has no interface, it is headless!
        private string colour; //Default colour of this scene
        private string debugHeader; //String to put infront of every debug message
        private const int headerSize = 22; //Debug console- size of header text

        private NetworkManager networkManager;
        private WorldManager worldManager;
        private VehicleManager vehicleManager;
        private PlayerManager playerManager;
        
        private SceneStatus status;

        private List<IGravityBody> gravityBodies = new List<IGravityBody>();
        private List<IPhysicsBody> physicsBodies = new List<IPhysicsBody>();
        private List<VehicleController> vehicles = new List<VehicleController>();
        #endregion;

        #region Properties
        public string Name { get; }
        public string Preffix { get { return preffix; } }
        public ApplicationRole Role { get { return role; } }
        public LayerMask Layer { get { return layer; } }
        public bool Headless { get { return headless; } }

        public SceneStatus Status { get { return status; } }
        public long NetworkIdentifier { get { return networkManager.Network.Identifier; } }

        public List<Planet> Worlds { get { return worldManager.Worlds; } }
        public List<IGravityBody> GravityBodies { get { return gravityBodies; } }
        public List<IPhysicsBody> PhysicsBodies { get { return physicsBodies; } }
        public List<VehicleController> Vehicles { get { return vehicles; } }

        public NetworkManager NetworkManager { get { return networkManager; } }
        public VehicleManager VehicleManager { get { return vehicleManager; } }
        public WorldManager WorldManager { get { return worldManager; } }
        public PlayerManager PlayerManager { get { return playerManager; } }
        #endregion

        #region Generator
        public void Initialize(string preffix, ApplicationRole role, string colour, string layerMask, bool headless)
        {
            status = SceneStatus.initializing;

            this.preffix = preffix;
            this.role = role;
            this.layer = LayerMask.NameToLayer(layerMask);
            UnityScene.SetLayerRecursively(this.gameObject, layer);
            this.headless = headless;
            this.colour = colour;

            debugHeader = "<b><color=" + colour + ">" + gameObject.name + "</color></b>: ";

            networkManager = gameObject.AddComponent<NetworkManager>();
            networkManager.Initialize(this);

            vehicleManager = gameObject.AddComponent<VehicleManager>();
            vehicleManager.Initialize(this);

            worldManager = gameObject.AddComponent<WorldManager>();
            worldManager.Initialize(this);

            if (!headless) {
                playerManager = gameObject.AddComponent<PlayerManager>();
                playerManager.Initialize(this);
            }

            status = SceneStatus.initializing;
            StartCoroutine(WaitForManagers());
        }

        private IEnumerator WaitForManagers()
        {
            while (status == SceneStatus.initializing) {

                bool playerManagerOk = headless;
                if (!headless) {
                    //We have a playerManager so check that.
                    playerManagerOk = (playerManager.Status == ManagerStatus.ready);
                }

                //Check all managers have finished their inits. 
                if (worldManager.Status == ManagerStatus.ready && 
                    networkManager.Status == ManagerStatus.ready && 
                    vehicleManager.Status == ManagerStatus.ready &&
                    playerManagerOk
                ) {
                    status = SceneStatus.postInit;
                    PostInitialization();
                }
                yield return new WaitForSeconds(1f);
            }
            yield return null;
        }

        /// <summary>
        /// All managers have declared they are ready for service- kick off game start
        /// </summary>
        private void PostInitialization()
        {
            status = SceneStatus.ready;
            Log("PostInitialization starting...", 12, "yellow");

            if (!Headless) {
                //We have an interface- spawn a player!
                string objectName = VehicleLibrary.instance.GetPrefabName(VehicleLibrary.instance.playerObject);

                if (objectName == null) return;

                GameObject playerObject = SpawnVehicle(objectName, worldManager.RandomWorld(), new Vector2(0f,0f)); //worldManager.RandomLatLon()
                if (playerObject) {
                    SetControlTarget(playerObject);
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Spawns a blank gameObject under the scene tree, with given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject SpawnGameObject(string name)
        {
            return (vehicleManager) ? vehicleManager.SpawnGameObject(name) : null;
        }

        /// <summary>
        /// Spawns a vehicle at given planet and latLon
        /// </summary>
        /// <param name="prefabName"></param>
        /// <param name="planet"></param>
        /// <param name="latLon"></param>
        /// <returns></returns>
        public GameObject SpawnVehicle(string prefabName, Planet planet, Vector2 latLon)
        {
            return (vehicleManager && planet) ? vehicleManager.SpawnVehicle(prefabName, planet, latLon) : null;
        }

        public void SetControlTarget(GameObject target)
        {
            playerManager.SetControlTarget(target);
        }

        public Planet GetNearestWorld(Vector3 worldPosition)
        {
            return (worldManager) ? worldManager.GetNearestWorld(worldPosition) : null;
        }
        #endregion


        #region Debugging
        private string LogParse(string message, string messageColour, bool header)
        {
            if (messageColour != "") {
                message = "<b><color=" + messageColour + ">" + message + "</color></b>";
            }
            if (header) {
                return "<size=" + headerSize + ">" + debugHeader + message + "</size>";
            }
            return debugHeader + message;
        }
        public void Log(string message, int priority = 1, string colour = "", bool header = false)
        {
            if (priority >= GameMaster.instance.LogThreshhold) {
                Debug.Log(LogParse(message, colour, header));
            }
        }
        public void LogWarning(string message, int priority = 7, string colour = "", bool header = false)
        {
            if (priority >= GameMaster.instance.LogThreshhold) {
                Debug.LogWarning(LogParse(message, colour, header));
            }
        }
        public void LogError(string message, int priority = 12, string colour = "", bool header = false)
        {
            //if (priority >= GameMaster.instance.LogThreshhold) {
                Debug.LogError(LogParse(message, colour, header));
            //}
        }
        #endregion
    }
}
