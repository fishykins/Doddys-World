using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;
using DW.Objects;
using IngameDebugConsole;

namespace DW.Network
{
    public class ServerInstance : INetwork
    {
        #region Variables
        //Public & Serialized

        //Private
        private NetPeerConfiguration netConfig;
        private NetServer server;
        private SceneInstance scene;
        private NetworkManager manager;
        private PacketHandler packetHandler;
        private StatusHandler statushandler;
        private ManagerStatus status = ManagerStatus.initializing;
        #endregion;

        #region Properties
        public NetServer Server { get { return server; } }
        public SceneInstance Scene { get { return scene; } }
        public int DebugLevel { get { return manager.debugLevel; } }
        public ManagerStatus Status { get { return status; } }
        public long Identifier { get { return Server.UniqueIdentifier; } }
        #endregion;

        #region Constructor
        /// <summary>
        /// Creates and starts the server. Maybe we should use a seperate method for start, but then when would we not want to start right away?
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="manager"></param>
        /// <param name="port"></param>
        public ServerInstance(SceneInstance scene, NetworkManager manager, int port)
        {
            this.manager = manager;
            this.scene = scene;

            packetHandler = new ClientPackets(this);
            statushandler = new StatusHandler(this);

            scene.Log("Starting server...", manager.debugLevel + 20, "yellow");

            //Fire up the server
            netConfig = new NetPeerConfiguration(GameMaster.instance.LidgrenID);
            netConfig.Port = port;

            server = new NetServer(netConfig);
            server.Start();

            scene.Log("Server status: " + server.Status.ToString(), manager.debugLevel);
            status = ManagerStatus.ready;

            DebugLogConsole.AddCommandInstance("ServerStatus", "Logs the net status", "LogServerNetworkStatus", this);
        }
        #endregion;

        #region Custom Methods
        /// <summary>
        /// Ends the server nice and tidy like
        /// </summary>
        public void Shutdown()
        {
            if (server != null)
            {
                server.Shutdown("byeee");
                scene.Log("Server has shutdown.", manager.debugLevel + 20, "orange");
            }
        }

        /// <summary>
        /// Called by NetworkManager
        /// </summary>
        public void NetworkUpdate()
        {
            if (server == null) return;
            HandleMessages();
        }

        public void NetworkTick()
        {
            if (scene.ObjectManager != null)
                HandleNetworkObjects();
        }

        private void HandleNetworkObjects()
        {
            //Send all our objects to the server!
            foreach (GameObject obj in scene.ObjectManager.Objects)
            {

                INetController controller = obj.GetComponent<INetController>();

                if (controller != null)
                {
                    NetOutgoingMessage message = server.CreateMessage(8);
                    manager.DumpNetControllerToMessage(controller, ref message);
                    server.SendToAll(message, NetDeliveryMethod.ReliableOrdered, 1);
                }
            }
        }

        // Takes incoming packets and throws them at the appropriate method for parsing.
        private void HandleMessages()
        {
            NetIncomingMessage message;
            while ((message = server.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        scene.Log(message.ReadString(), manager.debugLevel);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();
                        statushandler.HandleMessage(message, status);
                        break;
                    case NetIncomingMessageType.Data:
                        packetHandler.HandleMessage(message);
                        break;
                }
                server.Recycle(message);
            }
        }
        #endregion

        #region Debug Console
        public void LogServerNetworkStatus()
        {
            string info = "";
            info += "Status: " + server.Status.ToString() + "(Scroll for more info)\n";
            info += "NUID: " + Identifier + "\n";
            foreach (var connection in server.Connections)
            {
                info += "Conection: " + connection.RemoteUniqueIdentifier + "\n";
            }

            scene.Log(info);
        }
        #endregion
    }
}
