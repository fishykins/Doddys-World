﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Physics;

namespace DW.Vehicles
{
    public class HumanController : MonoBehaviour, IVehicleController
    {
        #region Variables
        //Public

        //Private
        private bool broadcastOverNet = true;
        private List<IPhysicsBody> physicsBodies = new List<IPhysicsBody>();
        private IInput input;
        private Rigidbody rb;
        private long origin = -1;
        private long host = -1;
        private int index = -1;
        private string prefab = "";
        private string uniqueIdentifier = "";
        private SceneInstance scene;
        private bool isLocal = false;
        #endregion

        #region Properties
        public long Origin { get { return origin; } }
        public long Host { get { return host; } }
        public string Prefab { get { return prefab; } }
        public string UniqueIdentifier { get { return uniqueIdentifier; } }
        public int Index { get { return index; } }
        public bool IsLocal { get { return isLocal; } }
        public SceneInstance Scene { get { return scene; } }
        public bool BroadcastOverNet { get { return broadcastOverNet; } set { broadcastOverNet = value; } }
        public List<IPhysicsBody> PhysicsBodies { get { return physicsBodies; } }
        public IInput Input { get { return input; } }
        public Transform Transform { get { return transform; } }
        #endregion

        #region Unity Methods

        #endregion

        #region Custom Methods
        public string Initialize(SceneInstance scene, long origin, string prefab, int index)
        {
            this.scene = scene;
            this.origin = origin;
            this.prefab = prefab;
            this.index = index;
            uniqueIdentifier = origin + "_" + index;

            scene.Log(uniqueIdentifier + " has initialized");

            return uniqueIdentifier;
        }

        public void SetInput(IInput input)
        {
            input = this.input;
        }

        public void SetHost(long newHost)
        {
            host = newHost;
            isLocal = (host == scene.NetworkIdentifier);

            if (physicsBodies != null)
            {
                //This vehicle has a body so we must determine its status
                foreach (MonoBehaviour body in physicsBodies)
                {
                    body.enabled = isLocal;
                }

            }

            if (rb)
            {
                rb.collisionDetectionMode = (isLocal) ? CollisionDetectionMode.ContinuousDynamic : CollisionDetectionMode.Discrete;
                //rb.interpolation = (isLocal) ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
                rb.detectCollisions = isLocal;
                rb.isKinematic = !isLocal;
            }
        }

        public void AddPhysicsBody(IPhysicsBody body)
        {
            physicsBodies.Add(body);
            body.Initialize(this);
            SetHost(host); //Will update body
        }
        #endregion
    }
}