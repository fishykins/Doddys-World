using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW
{
    [System.Serializable]
    public class Item
    {
        #region Variables
        //Public
        public string name;
        public string displayName;
        public string description;
        public Dictionary<string, int> stats = new Dictionary<string, int>();

        //Private
        private ushort id;
        #endregion
        
        #region Properties
        public int Index { get { return (int)id; } }
        #endregion

        public Item(int id, string name, string displayName, string description, Dictionary<string, int> stats)
        {
            this.id = (ushort)id;
            this.name = name;
            this.displayName = displayName;
            this.description = description;
            this.stats = stats;
        }
    }
}