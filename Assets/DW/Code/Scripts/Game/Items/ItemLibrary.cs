using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace DW
{
    public class ItemLibrary : MonoBehaviour
    {
        #region Variables
        //Public
        public static ItemLibrary instance;
        public GameObject worldItem;

        //Private
        private List<Item> items = new List<Item>();
        #endregion

        #region Properties
        public List<Item> Items { get { return items; } }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            if (ItemLibrary.instance == null)
                ItemLibrary.instance = this;

            CompileItems();
        }
        #endregion

        #region Custom Methods
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

        public Item GetItem(int id)
        {
            return items.Find(item => item.Index == id);
        }
        #endregion
    }
}