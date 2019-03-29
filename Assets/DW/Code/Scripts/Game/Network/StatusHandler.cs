using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

namespace DW.Network {
    // Server side handler that deals with connecting players, disconects and other fun stuff
	public class StatusHandler {
        #region Variables
        //Public & Serialized

        //Private
        private SceneInstance scene;
        private INetwork network;
        #endregion;

        #region Control Methods
        public StatusHandler(INetwork network)
        {
            this.network = network;
            this.scene = network.Scene;
        }


        /// <summary>
        /// Triggered by connection changes on the network, such as players joining/leaving
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        public void HandleMessage(NetIncomingMessage message, NetConnectionStatus status)
        {
            switch (status) {
                case NetConnectionStatus.Connected:
                    InitializeNewClient(message.SenderConnection);
                    scene.Log("Connetion Received from " + message.SenderConnection, network.DebugLevel);
                    break;
                case NetConnectionStatus.Disconnected:
                    scene.Log("Connetion dropped from " + message.SenderConnection, network.DebugLevel);
                    break;
                default:
                    scene.Log(message.SenderConnection + ": " + status + " (" + message.ReadString() + ")", network.DebugLevel);
                    break;
            }
        }
        #endregion

        #region Connection Methods
        private void InitializeNewClient(NetConnection connection)
        {
            //prep message
            NetOutgoingMessage MessageOut;
            MessageOut = connection.Peer.CreateMessage(8);
            MessageOut.Write((int)UniversalPacketType.ping);
            MessageOut.Write("Hello there");
            connection.SendMessage(MessageOut, NetDeliveryMethod.ReliableOrdered, 1);
        }
        #endregion
    }
}
