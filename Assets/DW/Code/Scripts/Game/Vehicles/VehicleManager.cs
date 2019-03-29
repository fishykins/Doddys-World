using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanetaryTerrain;
using DW.Physics;
using DW.Network;

namespace DW.Vehicles {
	public class VehicleManager : MonoBehaviour {
        #region Variables
        //Public & Serialized
        //[SerializeField, Range(-100, 100)]
        //private int debugLevel = 0;

        //Private
        private SceneInstance scene;
        private VehicleLibrary library;
        private NetworkManager networkManager;
        private ManagerStatus status = ManagerStatus.initializing;
        private int vehicleIndex = 0;

        private Dictionary<string, VehicleController> VehicleDictionary = new Dictionary<string, VehicleController>();
        #endregion;

        #region Properties
        public ManagerStatus Status { get { return status; } }
        #endregion;

        #region Unity Methods

        #endregion;

        #region Private Methods
        private void ParentToScene(GameObject obj)
        {
            if (scene.gameObject) {
                obj.transform.parent = scene.gameObject.transform;
                Fishy.UnityScene.SetLayerRecursively(obj, scene.Layer);
            }
        }
        #endregion

        #region Public Methods
        public void Initialize(SceneInstance scene)
        {
            this.scene = scene;
            library = VehicleLibrary.instance;
            networkManager = GetComponent<NetworkManager>();
            if (networkManager) {
                status = ManagerStatus.ready;
            } else {
                status = ManagerStatus.error;
            }
        }


        public GameObject SpawnGameObject(string name)
        {
            GameObject obj = new GameObject(name);
            ParentToScene(obj);
            return obj;
        }

        public bool UpdateVehicleFromNetwork(string nuid, Vector3 position, Vector3 rotation)
        {
            VehicleController networkVehicle;
            if (!VehicleDictionary.TryGetValue(nuid, out networkVehicle)) {
                //Spawn vehicle
                scene.Log("Requesting " + nuid + " from its localHost");
                return false;

            } else {
                networkVehicle.UpdateFromNetwork(position, rotation);
                return true;
            }
        }

        public bool TryGetNetworkVehicle(string uniqueIdentifier, out VehicleController networkVehicle)
        {
            return VehicleDictionary.TryGetValue(uniqueIdentifier, out networkVehicle);
        }

        private void InitVehicle(GameObject vehicle, string prefabName, bool initNetwork = true)
        {
            string text = "Initializing " + prefabName;
            if (initNetwork) text += " (Hosting)";
            scene.Log(text);

            vehicle.transform.name = prefabName;
            ParentToScene(vehicle);

            VehicleController netData = vehicle.GetComponent<VehicleController>();
            
            string uniqueIdentifier;

            //Default initialize and host object. 
            if (initNetwork || !netData) {
                if (!netData) {
                    //Vehicles need netData to function- add it!
                    netData = vehicle.AddComponent<VehicleController>();
                }

                uniqueIdentifier = netData.Initialize(scene, scene.NetworkIdentifier, prefabName, vehicleIndex); vehicleIndex++;
                netData.SetHost(scene.NetworkIdentifier);
            } else {
                //We have bypassed netinit, so simply get data as we assume its already been done
                uniqueIdentifier = netData.UniqueIdentifier;
            }

            IPhysicsBody[] bodies = netData.physicsBodies;

            scene.Vehicles.Add(netData);
            VehicleDictionary.Add(uniqueIdentifier, netData);

            if (bodies != null) {
                foreach (var body in bodies) {
                    body.Initialize(scene);
                    scene.PhysicsBodies.Add(body);
                }
            }
        }

        /// <summary>
        /// Spawns an object that is being hosted on a remote netowrk.
        /// </summary>
        /// <param name="prefabName"></param>
        /// <param name="origin"></param>
        /// <param name="index"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public GameObject SpawnFromNetwork(string prefabName, long origin, int vehicleIndex, long host)
        {
            GameObject prefab;

            if (!TryGetPrefab(prefabName, out prefab)) return null;
            if (VehicleDictionary.ContainsKey(origin + "_" + vehicleIndex)) return null;

            GameObject vehicle = Instantiate(prefab);

            //Set NetowrkVehicle data before we init, so we can overwrite default values
            VehicleController netData = vehicle.GetComponent<VehicleController>();
            if (!netData) netData = vehicle.AddComponent<VehicleController>();

            netData.Initialize(scene, origin, prefabName, vehicleIndex); vehicleIndex++;
            netData.SetHost(host);

            InitVehicle(vehicle, prefabName, false);

            return vehicle;
        }

        public GameObject SpawnVehicle(string prefabName)
        {
            GameObject prefab;

            if (!TryGetPrefab(prefabName, out prefab)) return null;

            GameObject vehicle = Instantiate(prefab);

            InitVehicle(vehicle, prefabName);

            return vehicle;
        }

        public GameObject SpawnVehicle(string prefabName, Planet planet, Vector2 latLon)
        {
            GameObject prefab;

            if (!TryGetPrefab(prefabName, out prefab)) return null;

            GameObject vehicle = planet.InstantiateOnPlanet(prefab, latLon, 100f);

            InitVehicle(vehicle, prefabName);

            return vehicle;
        }

        private bool TryGetPrefab(string prefabName, out GameObject prefab)
        {
            prefab = library.GetPrefab(prefabName);

            if (scene == null) {
                Debug.LogError("Cannot spawn vehicle- no scene set!");
                return false;
            }

            if (prefab == null) {
                scene.LogError("Cannot spawn vehicle " + prefabName + "- no prefab found!");
                return false;
            }

            return true;
        }
        #endregion
	}
}
