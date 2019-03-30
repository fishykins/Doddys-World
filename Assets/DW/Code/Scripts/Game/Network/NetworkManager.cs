﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;
using DW.Vehicles;
using IngameDebugConsole;

namespace DW.Network {
    /// <summary>
    /// The Network manager is in-charge of allocating jobs to server/client and executing update methods.
    /// </summary>
	public class NetworkManager : MonoBehaviour {
        #region Variables
        //Public
        public int debugLevel = 1;

        //Private
        private SceneInstance scene;
        private int port = 14242;
        private ManagerStatus status = ManagerStatus.initializing;
        private INetwork network;
        private float tickRate;
        #endregion;

        #region Properties
        public INetwork Network { get { return network; } }
        public ManagerStatus Status { get { return (network != null) ? network.Status : status; } }
        #endregion;

        #region Unity Methods
        private void Start()
        {
            tickRate = 1f / GameMaster.instance.TickRate;
            
        }

        void Update () {
            if (network != null) {
                network.NetworkUpdate();
            }
        }
        private void OnDestroy()
        {
            if (network != null) {
                network.Shutdown();
            }
        }
        #endregion;

        #region Custom Methods
        private IEnumerator PerformTick()
        {
            while (network != null) {
                network.NetworkTick();
                yield return new WaitForSeconds(tickRate);
            }
            yield return null;
        }


        public void Initialize(SceneInstance scene)
        {
            this.scene = scene;

            switch (scene.Role) {
                case ApplicationRole.host:
                    network = new ServerInstance(scene, this, port);
                    break;
                case ApplicationRole.client:
                    network = new ClientInstance(scene, this, "localhost", port);
                    break;
                case ApplicationRole.local:
                    break;
                default:
                    break;
            }

            StartCoroutine(PerformTick());
        }

        public void DumpVehicleToMessage(VehicleController vehicle, NetOutgoingMessage message)
        {
            Vector3 rotation = vehicle.transform.rotation.eulerAngles;

            message.Write(vehicle.UniqueIdentifier);
            message.Write(vehicle.Host);
            message.Write(vehicle.transform.position.x);
            message.Write(vehicle.transform.position.y);
            message.Write(vehicle.transform.position.z);
            message.Write(rotation.x);
            message.Write(rotation.y);
            message.Write(rotation.z);
        }
        #endregion
    }
}