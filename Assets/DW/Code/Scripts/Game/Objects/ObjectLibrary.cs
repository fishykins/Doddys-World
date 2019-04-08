using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DW.Objects {
	public class ObjectLibrary : MonoBehaviour {

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
        public static ObjectLibrary instance;

        [SerializeField]
        private NetObject[] networkObjects = new NetObject[0];
        public int playerObject = 0;

        //Private
        private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();
        private List<Item> items = new List<Item>();

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

        private void CompileItems()
        {
            items.Add(GenerateItemNull());

            string path = Application.streamingAssetsPath + "/items/items.json";
            string jsonString = File.ReadAllText(path);

            Item newItem = JsonUtility.FromJson<Item>(jsonString);
            
            items.Add(newItem);
        }

        private Item GenerateItemNull()
        {
            return new Item(0, "null", "nameNull", "description", null);
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

        public Item GetItem(int id)
        {
            return items.Find(item => item.Index == id);
        }
        #endregion
    }
}
