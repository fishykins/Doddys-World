using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Physics;
using Lidgren.Network;

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
        private IPhysicsBody body; //The main (and only) body we care about
        private Animator animator;
        private ICameraController cameraController;
        private HumanThirdPersonCam thirdPersonCam;
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
        public ICameraController CameraController { get { return cameraController; } }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            body = GetComponent<HumanBody>();
            animator = GetComponent<Animator>();
            thirdPersonCam = GetComponent<HumanThirdPersonCam>();

            if (thirdPersonCam)
            {
                cameraController = thirdPersonCam;
            }
            
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

            if (body != null)
            {
                physicsBodies.Add(body);
                body.Initialize(this);
            }

            scene.Log(uniqueIdentifier + " has initialized");

            return uniqueIdentifier;
        }

        public void SetInput(IInput input)
        {
            this.input = input;
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

            if (this.body == null) this.body = body;
            SetHost(host); //Will update body
        }

        public void PackNetworkMessage(ref NetOutgoingMessage message)
        {
            Vector3 rotation = transform.rotation.eulerAngles;

            message.Write(transform.position.x);
            message.Write(transform.position.y);
            message.Write(transform.position.z);
            message.Write(rotation.x);
            message.Write(rotation.y);
            message.Write(rotation.z);
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
        #endregion
    }
}