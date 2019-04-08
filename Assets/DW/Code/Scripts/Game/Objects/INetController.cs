using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

namespace DW.Objects
{
    /// <summary>
    /// This is required for an object to be network compatible. 
    /// All network related jobs are carried out here, in an effort to keep
    /// physics and network sepperated. This behaviour is apex, and will be able to 
    /// manipulate IO controllers, physicsbodies and rigidbodies.
    /// </summary>
    public interface INetController
    {
        SceneInstance Scene { get; }
        bool BroadcastOverNet { get; set; }
        long Origin { get; }
        long Host { get; }
        string Prefab { get; }
        string UniqueIdentifier { get; }
        int Index { get; }
        bool IsLocal { get; }

        string Initialize(SceneInstance scene, long origin, string prefab, int index);
        bool PackNetworkMessage(ref NetOutgoingMessage message);
        void UnpackNetworkMessage(NetIncomingMessage message);
        void SetHost(long newHost);
    }
}