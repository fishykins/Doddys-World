using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Objects;

namespace DW.Physics
{
    /// <summary>
    /// The PhysicsBody is above the rigidbody component in the heirachy, and bellow everything else.
    /// It will carry out fixedUpdates to deal with all physics related tasks on the object.
    /// For input, listen to the IOController, and also sometimes the NetController (which is top of the food chain)
    /// </summary>
    public interface IPhysicsBody
    {
        void Initialize(SceneInstance scene);
        void Initialize(IIOController controller);
        SceneInstance Scene { get; }
        Transform Transform { get; }
        IIOController Controller { get; set;}
        void Enable(bool enabled);
    }
}
