using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlanetaryTerrain;
using DW.Physics;
using DW.Network;

namespace DW.Vehicles
{
    public class VehicleManager : MonoBehaviour
    {
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

        private Dictionary<string, IVehicleController> vehicleDictionary = new Dictionary<string, IVehicleController>();
        #endregion;

        #region Properties
        public ManagerStatus Status { get { return status; } }
        #endregion;

        #region Unity Methods

        #endregion;

        #region Private Methods
        private void InitVehicle(GameObject vehicle, string prefabName, bool initNetwork = true)
        {
            vehicle.transform.name = prefabName;
            ParentToScene(vehicle);

            IVehicleController controller = vehicle.GetComponent<IVehicleController>();

            string uniqueIdentifier;

            //Default initialize and host object. 
            if (initNetwork || controller == null)
            {
                if (controller == null)
                {
                    //Vehicles need netData to function- add it!
                    controller = (IVehicleController)vehicle.AddComponent<VehicleController>();
                }

                uniqueIdentifier = controller.Initialize(scene, scene.NetworkIdentifier, prefabName, vehicleIndex); vehicleIndex++;
                controller.SetHost(scene.NetworkIdentifier);
            }
            else
            {
                //We have bypassed netinit, so simply get data as we assume its already been done
                uniqueIdentifier = controller.UniqueIdentifier;
            }

            scene.Vehicles.Add(controller);
            vehicleDictionary.Add(uniqueIdentifier, controller);
        }
        private void ParentToScene(GameObject obj)
        {
            if (scene.gameObject)
            {
                obj.transform.parent = scene.gameObject.transform;
                Unifish.UnityScene.SetLayerRecursively(obj, scene.Layer);
            }
        }
        #endregion

        #region Public Methods
        public void Initialize(SceneInstance scene)
        {
            this.scene = scene;
            library = VehicleLibrary.instance;
            networkManager = GetComponent<NetworkManager>();
            if (networkManager)
            {
                status = ManagerStatus.ready;
            }
            else
            {
                status = ManagerStatus.error;
            }
        }


        public GameObject SpawnGameObject(string name)
        {
            GameObject obj = new GameObject(name);
            ParentToScene(obj);
            return obj;
        }

        public bool TryGetVehicle(string uniqueIdentifier, out IVehicleController networkVehicle)
        {
            return vehicleDictionary.TryGetValue(uniqueIdentifier, out networkVehicle);
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
            if (vehicleDictionary.ContainsKey(origin + "_" + vehicleIndex)) return null;

            GameObject vehicle = Instantiate(prefab);

            //Set NetowrkVehicle data before we init, so we can overwrite default values
            IVehicleController controller = vehicle.GetComponent<IVehicleController>();
            if (controller == null) controller = (IVehicleController)vehicle.AddComponent<VehicleController>();

            controller.Initialize(scene, origin, prefabName, vehicleIndex); vehicleIndex++;
            controller.SetHost(host);

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

            if (scene == null)
            {
                Debug.LogError("Cannot spawn vehicle- no scene set!");
                return false;
            }

            if (prefab == null)
            {
                scene.LogError("Cannot spawn vehicle " + prefabName + "- no prefab found!");
                return false;
            }

            return true;
        }
        #endregion
    }
}
