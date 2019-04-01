using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Vehicles;

namespace DW.Physics
{
    public interface IPhysicsBody
    {
        void Initialize(SceneInstance scene);
        void Initialize(IVehicleController controller);
        SceneInstance Scene { get; }
        Transform Transform { get; }
        IVehicleController Controller { get; set;}
    }
}
