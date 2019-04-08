using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Physics;
using Lidgren.Network;

namespace DW.Objects
{
    /// <summary>
    /// This interface passively represents any kind of inputs and outputs an object may have. 
    /// Typically this is player/AI input, camera controllers and other such things.
    /// It does not call anything on any other mono, and is not required to make a functional object
    /// </summary>
    public interface IIOController
    {
        SceneInstance Scene { get; }
        List<IPhysicsBody> PhysicsBodies { get; }
        IInput Input { get; }
        Transform Transform { get; }
        ICameraController CameraController { get; }

        void Initialize(SceneInstance scene);
        void SetInput(IInput input);
    }
}