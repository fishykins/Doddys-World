using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;
using DW.Objects;

namespace DW.Network
{
    /// <summary>
    /// Deals with incoming packets.
    /// </summary>
	public class PacketHandler
    {
        #region Variables
        //Public & Serialized

        //Private
        protected SceneInstance scene;
        protected INetwork network;
        protected int debuggingLevel = 1;

        protected delegate void Packet_(NetIncomingMessage message);
        protected Dictionary<int, Packet_> packetDictionary = new Dictionary<int, Packet_>();

        #region Packet Unique Variables
        private List<string> requestedVehicles = new List<string>();
        #endregion

        #endregion;

        #region Control Methods
        protected virtual void InitializePackets()
        {
            packetDictionary.Add((int)UniversalPacketType.ping, P_Ping);
            packetDictionary.Add((int)UniversalPacketType.pong, P_Pong);
            packetDictionary.Add((int)UniversalPacketType.RequestVehicle, P_VehicleRequest);
            packetDictionary.Add((int)UniversalPacketType.VehicleDNA, P_VehicleDNA);
            packetDictionary.Add((int)UniversalPacketType.vehicleUpdate, P_VehicleUpdate);
        }

        /// <summary>
        /// Takes a message and invokes the correct method for it
        /// </summary>
        /// <param name="message"></param>
        public virtual void HandleMessage(NetIncomingMessage message)
        {
            int messageType = message.ReadInt32();
            Packet_ packet;
            if (packetDictionary.TryGetValue(messageType, out packet))
            {
                packet.Invoke(message);
            }
            else
            {
                scene.LogError("Message type '" + messageType + "' not found- make sure it has a method and is included in the init method", 10);
            }
        }
        #endregion

        #region Universal Packet Methods
        //These packets can be received by both server and client

        private void P_Ping(NetIncomingMessage message)
        {
            string text = message.ReadString();
            scene.Log("Ping: " + text, network.DebugLevel + 5);

            //prep message
            NetOutgoingMessage MessageOut;
            MessageOut = message.SenderConnection.Peer.CreateMessage(8);
            MessageOut.Write((int)UniversalPacketType.pong);
            MessageOut.Write("General Kenobi");
            message.SenderConnection.SendMessage(MessageOut, NetDeliveryMethod.ReliableOrdered, 1);
        }

        private void P_Pong(NetIncomingMessage message)
        {
            string text = message.ReadString();
            scene.Log("Pong: " + text, network.DebugLevel + 5);
        }

        /// <summary>
        /// Returns DNA for requested vehicle
        /// </summary>
        /// <param name="message"></param>
        private void P_VehicleRequest(NetIncomingMessage message)
        {
            string nuid = message.ReadString();
            scene.Log(nuid + " was requested");

            INetController networkVehicle;
            if (scene.ObjectManager.TryGetNetVehicle(nuid, out networkVehicle))
            {
                //prep message
                NetOutgoingMessage MessageOut;
                MessageOut = message.SenderConnection.Peer.CreateMessage(5);
                MessageOut.Write((int)UniversalPacketType.VehicleDNA);
                MessageOut.Write(networkVehicle.Origin);
                MessageOut.Write(networkVehicle.Host);
                MessageOut.Write(networkVehicle.Index);
                MessageOut.Write(networkVehicle.Prefab);

                message.SenderConnection.SendMessage(MessageOut, NetDeliveryMethod.ReliableOrdered, 1);
            }
        }

        /// <summary>
        /// Vehicle DNA!
        /// </summary>
        /// <param name="message"></param>
        private void P_VehicleDNA(NetIncomingMessage message)
        {
            long origin = message.ReadInt64();
            long host = message.ReadInt64();
            int index = message.ReadInt32();
            string prefab = message.ReadString();

            scene.ObjectManager.SpawnFromNetwork(prefab, origin, index, host);
        }

        /// <summary>
        /// Update a vehicle position and relay to other clients
        /// </summary>
        /// <param name="message"></param>
        private void P_VehicleUpdate(NetIncomingMessage message)
        {
            long host = message.ReadInt64();

            //If we own this vehicle, ignore this message
            if (host == scene.NetworkIdentifier) return;

            string uniqueIdentifier = message.ReadString();

            INetController networkVehicle;
            if (scene.ObjectManager.TryGetNetVehicle(uniqueIdentifier, out networkVehicle))
            {
                //update vehicle
                networkVehicle.UnpackNetworkMessage(message);
            }
            else
            {
                if (!requestedVehicles.Contains(uniqueIdentifier))
                {
                    //Request vehicle
                    NetOutgoingMessage messageReturn;
                    messageReturn = message.SenderConnection.Peer.CreateMessage(2);
                    messageReturn.Write((int)UniversalPacketType.RequestVehicle);
                    messageReturn.Write(uniqueIdentifier);

                    message.SenderConnection.SendMessage(messageReturn, NetDeliveryMethod.ReliableOrdered, 1);

                    requestedVehicles.Add(uniqueIdentifier);
                }
            }
        }
        #endregion
    }
}
