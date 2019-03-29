using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

namespace DW.Network {
    /// <summary>
    /// A class that contains all message types and assosiated methods that the client can send. Will be created on serverInstance!
    /// </summary>
	public class ClientPackets : PacketHandler
    {
        public ClientPackets(INetwork network)
        {
            this.network = network;
            this.scene = network.Scene;
            
            InitializePackets();
        }

        protected override void InitializePackets()
        {
            base.InitializePackets();
            
        }

        #region Packet Methods
        
        #endregion
    }
}
