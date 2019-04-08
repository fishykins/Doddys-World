using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;
using DW.Physics;

namespace DW.Objects
{
    public class HumanNetController : MonoBehaviour, INetController
    {
        #region Variables
        private bool broadcastOverNet = true;
        private long origin = -1;
        private long host = -1;
        private int index = -1;
        private string prefab = "";
        private string uniqueIdentifier = "";
        private SceneInstance scene;
        private bool isLocal = false;
        private Rigidbody rb;
        private IIOController ioController;
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

        #endregion

        #region Unity Methods
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            ioController = GetComponent<IIOController>();
        }
        #endregion

        #region Custom Methods
        public string Initialize(SceneInstance scene, long origin, string prefab, int index)
        {
            this.scene = scene;
            this.origin = origin;
            this.prefab = prefab;
            this.index = index;
            uniqueIdentifier = origin + "_" + index;

            return uniqueIdentifier;
        }

        public bool PackNetworkMessage(ref NetOutgoingMessage message)
        {
            Vector3 rotation = transform.rotation.eulerAngles;

            message.Write(transform.position.x);
            message.Write(transform.position.y);
            message.Write(transform.position.z);
            message.Write(rotation.x);
            message.Write(rotation.y);
            message.Write(rotation.z);
            return true;
        }

        public void UnpackNetworkMessage(NetIncomingMessage message)
        {
            if (!rb) return;

            float x = message.ReadFloat();
            float y = message.ReadFloat();
            float z = message.ReadFloat();
            float xRot = message.ReadFloat();
            float yRot = message.ReadFloat();
            float zRot = message.ReadFloat();

            rb.MovePosition(new Vector3(x, y, z));
            rb.MoveRotation(Quaternion.Euler(new Vector3(xRot, yRot, zRot)));
        }

        public void SetHost(long newHost)
        {
            host = newHost;
            isLocal = (host == scene.NetworkIdentifier);

            if (ioController != null)
            {
                if (ioController.PhysicsBodies != null)
                {
                    foreach (IPhysicsBody pb in ioController.PhysicsBodies)
                    {
                        pb.Enable(isLocal);
                    }
                }
            }

            if (rb)
            {
                rb.collisionDetectionMode = (isLocal) ? CollisionDetectionMode.ContinuousDynamic : CollisionDetectionMode.Discrete;
                rb.detectCollisions = isLocal;
                rb.isKinematic = !isLocal;
            }
        }
        #endregion
    }
}