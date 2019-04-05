using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW
{
    /// <summary>
    /// an ItemInstance is a class that represents a realworld item, be it in cargo or worldSpace.
    /// </summary>
    public class ItemInstance
    {
        public Item classType;
        public ItemType type;
        public int health = 100;
        
        
    }
}