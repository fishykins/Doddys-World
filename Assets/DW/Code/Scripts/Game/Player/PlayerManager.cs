using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Physics;
using DW.Objects;

namespace DW.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerManager : MonoBehaviour
    {
        #region Variables
        //Public & Serialized

        //Private
        private SceneInstance scene;
        private ManagerStatus status;

        private IIOController vehicleController;
        private ICameraController cameraController;
        private IInput input;
        private bool interfaceOpen = false;
        private InteractionMenu interactionMenu;
        #endregion;

        #region Properties
        public ManagerStatus Status { get { return status; } }
        public IIOController ControlBody { get { return vehicleController; } }
        public IInput Input { get { return input; } }
        #endregion;

        #region Unity Methods
        private void Update()
        {
            HandleInterface();
        }
        private void LateUpdate()
        {
            if (cameraController != null)
                cameraController.UpdateCamera();
        }
        #endregion;

        #region Custom Methods
        public void Initialize(SceneInstance scene)
        {
            this.scene = scene;
            input = GetComponent<PlayerController>();
            status = ManagerStatus.ready;
        }

        /// <summary>
        /// Assignes player input to vehicle.
        /// </summary>
        /// <param name="target"></param>
        public void SetControlTarget(GameObject target)
        {
            IIOController controller = target.GetComponent<IIOController>();

            if (controller == null)
            {
                scene.LogError("SetControlTarget was just passed an object that does not have a VehicleController");
                return;
            }

            if (controller.Scene != scene)
            {
                scene.LogError("SetControlTarget was just passed an object from '" + controller.Scene.name + "'- this shouldn't happen!");
                return;
            }


            //Check if we need to untarget old target
            if (vehicleController != null)
            {
                vehicleController.SetInput(null);
            }

            //Set and apply new controller
            vehicleController = controller;
            vehicleController.SetInput(input);

            if (vehicleController.CameraController != null)
            {
                cameraController = vehicleController.CameraController;
                cameraController.AssignCamera(Camera.main.transform);
            }
            else
            {
                cameraController = null;
                scene.LogWarning(vehicleController.Transform.name + " does not have a camera controller- camera will not uptdate!");
            }

            scene.Log("Control target set to " + vehicleController.Transform.name);
        }

        private void HandleInterface()
        {
            if (input.Interaction)
            {
                //We need an interaction menu open- stat!
                if (!interfaceOpen)
                    OpenInterface();
            }
            else
            {
                if (interfaceOpen)
                    CloseInterface();
            }
        }

        private void OpenInterface()
        {
            interfaceOpen = true;
            interactionMenu = new InteractionMenu();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            scene.Log("Opened interface");
        }

        private void CloseInterface()
        {
            interactionMenu.Destroy();
            interfaceOpen = false;
            interactionMenu = null;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            scene.Log("closed interface");
        }
        #endregion
    }
}
