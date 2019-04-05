using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW.InteractionMenu
{
    public interface IMenuItem
    {
        string condition { get; }
        string Action { get; }
        int parent { get; set; }
    }
}