using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

namespace DW.Network {
    /// <summary>
    /// A class that contains all message types and assosiated methods that the server can send. Will be created on clientInstance!
    /// </summary>
	public class ServerPackets : PacketHandler
    {
        public ServerPackets(INetwork network)
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
        private void P_OnConnect(NetIncomingMessage message)
        {
            string text = message.ReadString();
            
        }
        #endregion
    }
}
