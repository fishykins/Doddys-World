using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW.Vehicles {
	public class VehicleLibrary : MonoBehaviour {

        #region Classes
        [System.Serializable]
        public class NetObject
        {
            public string name;
            public GameObject netObject;
        }
        #endregion

        #region Variables
        //Public & Serialized
        public static VehicleLibrary instance;

        [SerializeField]
        private NetObject[] networkObjects = new NetObject[0];
        public int playerObject = 0;

        //Private
        private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

        #endregion;

        #region Properties

        #endregion;

        #region Unity Methods
        private void Awake()
        {
            if (instance == null) {
                instance = this;
                CompilePrefabs();
            }
        }
        #endregion;

        #region Custom Methods
        private void CompilePrefabs()
        {
            foreach (NetObject netobj in networkObjects) {
                prefabDictionary.Add(netobj.name, netobj.netObject);
            }
        }

        public GameObject GetPrefab(string name)
        {
            GameObject prefab = null;
            if (prefabDictionary.TryGetValue(name, out prefab)) {
                return prefab;
            }
            return null;
        }

        public string GetPrefabName(int index)
        {
            if (index < networkObjects.Length) {
                return networkObjects[index].name;
            }

            Debug.LogError("Cant find vehicle prefab index- outside of range (" + networkObjects.Length + ", " + index + ")");
            return null;
        }
        #endregion
    }
}
