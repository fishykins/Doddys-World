﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DW.Physics;
using PlanetaryTerrain;

namespace DW.Objects
{
    //The base PhysicsBody handles simplistic stuff, such as finding world data and calculating gravity. Controllers are not nescisary for these bois
    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("Physics/Physics Body")]
    public class PhysicsBody : MonoBehaviour, IPhysicsBody
    {
        #region Variables
        //Public & Serialized
        public Vector3 debug;
        public bool applyGravity = true;

        //Private
        protected Rigidbody rb;
        protected SceneInstance scene;
        protected Planet nearestWorld;
        protected IIOController controller;

        #endregion;

        #region Properties
        public SceneInstance Scene { get { return scene; } }
        public Transform Transform { get { return transform; } }
        public IIOController Controller { get { return controller; } set { controller = value; } }
        #endregion;

        #region Unity Methods
        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            if (scene)
                nearestWorld = scene.GetNearestWorld(transform.position);
        }

        protected virtual void FixedUpdate()
        {
            if (rb && nearestWorld)
                HandleWorldPhysics();
        }
        #endregion;

        #region Custom Methods
        protected virtual void HandleWorldPhysics()
        {
            if (applyGravity)
                HandleGravity();

            HandleFallingThroughWorld(nearestWorld);
        }

        /// <summary>
        /// Initializes with just a scene. Used for objects that dont have IO controllers
        /// </summary>
        /// <param name="scene"></param>
        public void Initialize(SceneInstance scene)
        {
            this.scene = scene;
        }

        /// <summary>
        /// Initializes as a slave to an IO controller.
        /// </summary>
        /// <param name="controller"></param>
        public void Initialize(IIOController controller)
        {
            this.controller = controller;
            this.scene = controller.Scene;
        }

        private void HandleGravity()
        {
            Vector3 finalGravity = Vector3.zero;
            foreach (IGravityBody gravityObject in scene.GravityBodies)
            {
                finalGravity += gravityObject.GetGravitationalForce(transform.position, rb.mass);
            }

            debug = finalGravity;

            if (finalGravity != Vector3.zero)
                rb.AddForce(finalGravity);
        }


        private void HandleFallingThroughWorld(Planet world)
        {
            if ((transform.position - world.transform.position).magnitude < world.radius * 0.8f)
            {
                //We are at less than %80 of the worlds radius- lets assume we have fallen through and fix...
                float height = world.HeightAtXYZ(transform.position / world.radius);
            }
        }

        public void Enable(bool enabled)
        {
            this.enabled = enabled;
        }
        #endregion
    }
}
