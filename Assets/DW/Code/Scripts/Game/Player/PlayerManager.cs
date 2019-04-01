using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Physics;
using DW.Vehicles;

namespace DW.Player {
    [RequireComponent(typeof(PlayerController))]
	public class PlayerManager : MonoBehaviour {
        #region Variables
        //Public & Serialized

        //Private
        private SceneInstance scene;
        private ManagerStatus status;

        private IVehicleController controlBody;
        private IInput input;
        #endregion;

        #region Properties
        public ManagerStatus Status { get { return status; } }
        public IVehicleController ControlBody { get { return controlBody; } }
        public IInput Input { get { return input; } }
        #endregion;

        #region Unity Methods

        #endregion;

        #region Custom Methods
        public void Initialize(SceneInstance scene)
        {
            this.scene = scene;
            input = GetComponent<PlayerController>();
            status = ManagerStatus.ready;
        }



        public void SetControlTarget(GameObject target)
        {
            IVehicleController controller = target.GetComponent<IVehicleController>(); 

            if (controller == null) {
                scene.LogError("SetControlTarget was just passed an object that does not have a VehicleController");
                return;
            }

            if (controller.Scene != scene) {
                scene.LogError("SetControlTarget was just passed an object from '" + controller.Scene.name + "'- this shouldn't happen!");
                return;
            }


            //Check if we need to untarget old target
            if (controlBody != null) {
                controlBody.SetInput(null);
            }

            controlBody = controller;

            controller.SetInput(input);

            scene.Log(target.gameObject.name + " has been set to player input");

            if (controller.Transform) {
                Camera.main.transform.SetParent(controller.Transform);
                Camera.main.transform.localPosition = new Vector3(0f, 3f, -4f);
                Camera.main.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            }
        }
        #endregion
    }
}
