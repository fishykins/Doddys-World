using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;
using DW.Vehicles;
using IngameDebugConsole;

namespace DW.Network {
	public class ClientInstance : INetwork
    {
        #region Variables
        //Public & Serialized

        //Private
        private NetPeerConfiguration netConfig;
        private NetClient client;
        private SceneInstance scene;
        private NetworkManager manager;
        private PacketHandler packetHandler;
        private ManagerStatus status = ManagerStatus.initializing;

        #endregion;

        #region Properties
        public NetClient Client { get { return client; } }
        public SceneInstance Scene { get { return scene; } }
        public int DebugLevel { get { return manager.debugLevel; } }
        public ManagerStatus Status { get { return status; } }
        public long Identifier { get { return client.UniqueIdentifier; } }
        #endregion;

        #region Constructor
        public ClientInstance(SceneInstance scene, NetworkManager manager, string host, int port)
        {
            this.manager = manager;
            this.scene = scene;

            packetHandler = new ServerPackets(this);

            scene.Log("Starting client...", manager.debugLevel + 20, "yellow");

            netConfig = new NetPeerConfiguration(GameMaster.instance.LidgrenID);
            client = new NetClient(netConfig);
            client.Start();

            ConnectToServer(host, port);

            scene.Log("Client status: " + client.Status.ToString(), manager.debugLevel);
            status = ManagerStatus.ready;

            DebugLogConsole.AddCommandInstance("NetStatus", "Logs the net status", "LogClientNetworkStatus", this);
        }
        #endregion;

        #region Custom Methods
        /// <summary>
        /// Connects the client to server
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public void ConnectToServer(string host, int port)
        {
            scene.Log("Joining server '" + host + "' on port " + port, manager.debugLevel);
            client.Connect(host, port);
        }

        // Called by NetworkManager
        public void NetworkUpdate()
        {
            HandleMessages();
        }

        //Called by netowrk manager at a set tickrate, for sending data.
        public void NetworkTick()
        {
            if (client.ServerConnection != null) {
                HandleNetworkObjects();
            }
        }

        public void Shutdown()
        {

        }

        //Handles all incoming messages
        private void HandleMessages()
        {
            NetIncomingMessage message;
            while ((message = client.ReadMessage()) != null) {
                switch (message.MessageType) {
                    case NetIncomingMessageType.Data:
                        packetHandler.HandleMessage(message);
                        break;
                }
                client.Recycle(message);
            }
        }

        private void HandleNetworkObjects()
        {
            //Send all our objects to the server!
            foreach (VehicleController vehicle in scene.Vehicles) 
            {
                if (vehicle.Host == client.UniqueIdentifier) {

                    NetOutgoingMessage message = client.ServerConnection.Peer.CreateMessage(9);
                    message.Write((int)UniversalPacketType.vehicleUpdate);

                    manager.DumpVehicleToMessage(vehicle, message);

                    client.ServerConnection.SendMessage(message, NetDeliveryMethod.ReliableOrdered, 1);
                }
            }
        }
        #endregion

        #region Debug Console
        public void LogClientNetworkStatus()
        {
            string info = "";
            info += "Status: " + client.ConnectionStatus.ToString() + "(Scroll for more data)\n";
            info += "Local ID: " + Identifier + "\n";
            if (client.ServerConnection != null) {
                info += "Server ID: " + client.ServerConnection.RemoteUniqueIdentifier + "\n";
            }
            else {
                info += "No Server conection!" + "\n";
            }

            scene.Log(info);
        }
        #endregion
    }
}
