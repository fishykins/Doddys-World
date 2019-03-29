using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren;

namespace DW.Network {
    //Interface that applies to both client and server
	public interface INetwork {
        SceneInstance Scene { get; }
        int DebugLevel { get; }
        ManagerStatus Status { get; }
        long Identifier { get; }

        void NetworkUpdate();
        void NetworkTick();
        void Shutdown();

        
	}
}
