using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using DW.Physics;

namespace DW.Vehicles {
    //This script controls a vehicle as a generic holder of all things data related. It handles input and network primarily
	public class VehicleController : MonoBehaviour {
        #region Variables
        //Public & Serialized
        public bool broadcastOverNet = true;
        public IPhysicsBody[] physicsBodies;
        public IInput input;
        public Transform cameraRoot;

        //Private
        private Rigidbody rb;
        private long origin = -1;
        private long host = -1;
        private int index = -1;
        private string prefab = "";
        private string uniqueIdentifier = "";
        
        private SceneInstance scene;
        private bool isLocal = false;

        #endregion;

        #region Properties
        public long Origin { get { return origin; } }
        public long Host { get { return host; } }
        public string Prefab { get { return prefab; } }
        public string UniqueIdentifier { get { return uniqueIdentifier; } }
        public int Index { get { return index; } }
        public bool IsLocal { get { return isLocal; } }
        public SceneInstance Scene { get { return scene; } }
        #endregion;

        #region Unity Methods
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            BuildObject();

            if (!cameraRoot)
                cameraRoot = transform;
        }
        #endregion;

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

        public void SetHost(long newHost)
        {
            host = newHost;
            isLocal = (host == scene.NetworkIdentifier);

            if (physicsBodies != null) {
                //This vehicle has a body so we must determine its status
                foreach (MonoBehaviour body in physicsBodies) {
                    body.enabled = isLocal;
                }
                
            }

            if (rb) {
                rb.collisionDetectionMode = (isLocal) ? CollisionDetectionMode.ContinuousDynamic : CollisionDetectionMode.Discrete;
                //rb.interpolation = (isLocal) ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
                rb.detectCollisions = isLocal;
                rb.isKinematic = !isLocal;
            }
        }

        public void UpdateFromNetwork(Vector3 position, Vector3 rotation)
        {
            if (rb) {
                rb.MovePosition(position);
                rb.MoveRotation(Quaternion.Euler(rotation));
            } else {
                transform.position = position;
                transform.rotation = Quaternion.Euler(rotation);
            }
        }

        public void BuildObject()
        {
            physicsBodies = gameObject.GetComponentsInChildren<IPhysicsBody>();
        }

        public void SetInput(IInput input)
        {
            this.input = input;
            foreach (var pb in physicsBodies) {
                pb.Controller = input;
            }
        }

        public void RemoveInput()
        {
            SetInput(null);
        }
        #endregion
    }
}
