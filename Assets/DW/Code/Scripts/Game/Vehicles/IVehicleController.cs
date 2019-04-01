using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Physics;
using Lidgren.Network;

namespace DW.Vehicles
{
    public interface IVehicleController
    {
        bool BroadcastOverNet { get; set; }
        List<IPhysicsBody> PhysicsBodies { get; }
        IInput Input { get; }
        long Origin { get; }
        long Host { get; }
        string Prefab { get; }
        string UniqueIdentifier { get; }
        int Index { get; }
        bool IsLocal { get; }
        SceneInstance Scene { get; }
        Transform Transform { get; }

        string Initialize(SceneInstance scene, long origin, string prefab, int index);
        void SetInput(IInput input);
        void SetHost(long newHost);
        void AddPhysicsBody(IPhysicsBody body);

        void PackNetworkMessage(ref NetOutgoingMessage message);
        void UnpackNetworkMessage(NetIncomingMessage message);
    }
}