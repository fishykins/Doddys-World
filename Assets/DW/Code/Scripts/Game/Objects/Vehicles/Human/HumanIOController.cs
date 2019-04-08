using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Scripting.Hosting;
using UnityEngine;
using Lidgren.Network;
using IronPython.Hosting;
using DW.Physics;

namespace DW.Objects
{
    public class HumanIOController : MonoBehaviour, IIOController
    {
        #region Variables
        //Public

        //Private
        private IInput input;
        private Rigidbody rb;
        private dynamic pythoninputFunc;
        private bool hasInputFunction = false;

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

            //Python
            string path = Application.persistentDataPath + "/Custom Scripts/human.py";
            if (File.Exists(path))
            {
                string code = File.ReadAllText(path);
                ScriptEngine pythonEngine = global::UnityPython.CreateEngine(new string[] { "UnityEngine", "DW" });
                ScriptSource pythonSource = pythonEngine.CreateScriptSourceFromString(code);
                ScriptScope pythonScope = pythonEngine.CreateScope();
                pythonSource.Execute(pythonScope);
                hasInputFunction = pythonScope.TryGetVariable("Fnc_Input", out pythoninputFunc);
            }
        }

        private void Update()
        {
            if (input != null && hasInputFunction)
            {
                var result = pythoninputFunc(input.XAxis, input.YAxis, input.ZAxis);
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