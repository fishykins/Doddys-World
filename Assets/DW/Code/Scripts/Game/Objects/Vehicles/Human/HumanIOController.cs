using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Physics;
using Lidgren.Network;

namespace DW.Objects
{
    public class HumanIOController : MonoBehaviour, IIOController
    {
        #region Variables
        //Public

        //Private
        private IInput input;
        private Rigidbody rb;

        private SceneInstance scene;
        private IPhysicsBody body; //The main (and only) body we care about
        private Animator animator;
        private ICameraController cameraController;
        private HumanThirdPersonCam thirdPersonCam;
        #endregion

        #region Properties
        public SceneInstance Scene { get { return scene; } }
        public List<IPhysicsBody> PhysicsBodies { get { return new List<IPhysicsBody> { body }; } }
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
        public void Initialize(SceneInstance scene)
        {
            this.scene = scene;
            foreach (var pb in PhysicsBodies)
            {
                pb.Initialize(this);
            }
        }

        public void SetInput(IInput input)
        {
            this.input = input;
        }
        #endregion
    }
}