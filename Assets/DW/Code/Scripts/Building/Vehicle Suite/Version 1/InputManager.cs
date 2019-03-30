using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unifish.GridsOld;

namespace DW.Building.DepreciatedShipSuite
{
	public class InputManager : MonoBehaviour {
        #region Variables
        //Public & Serialized
        [Header("Control Settings")]
        [SerializeField]
        private Camera mainCamera = null;
        [SerializeField]
        private Transform mainLight = null;
        [SerializeField]
        private GameObject pointerPrefab = null;
        [SerializeField]
        private float cameraRange = 20f;
        [SerializeField]
        private bool cameraHasLight = true;
        [SerializeField]
        private Material selectionMaterial = null;
        [SerializeField]
        private Material demoMaterial = null;

        //Private
        private ShipManager shipManager;
        private BuildManager buildManager;
        private Transform pointer;
        private Transform demoBlock;
        private DynamicShape currentShape;

        private Vector3 pointerTarget;
        private Material pointerMainMaterial;

        private DynamicVertex refVertex;
        private DynamicShape selectedDeformShape;
        private List<DynamicShape> selectedShapes = new List<DynamicShape>();
        #endregion;

        #region Properties

        #endregion;


        #region Unity Methods
        private void Awake()
        {
            shipManager = GetComponent<ShipManager>();
            buildManager = GetComponent<BuildManager>();
            if (pointerPrefab) {
                pointer = Instantiate(pointerPrefab).transform;
                pointer.name = "Pointer";
                pointer.SetParent(transform);
                pointerMainMaterial = pointer.GetComponent<Renderer>().material;
            }

            if (buildManager.StandardShape) {
                demoBlock = Instantiate(buildManager.StandardShape).transform;
                demoBlock.name = "DemoBlock";
                demoBlock.SetParent(transform);
                demoBlock.GetComponent<Renderer>().material = demoMaterial;
                demoBlock.GetComponent<DynamicShape>().BuildBasicCube(Vector3.zero, buildManager.BlockScale, false);
            }
        }

        private void Start()
        {
            ToggleLightLock();
        }

        private void Update()
        {
            pointerTarget = Vector3.zero;

            HandleInput();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (UnityEngine.Physics.Raycast(ray, out hit, cameraRange)) {
                HandleRayTarget(hit);
            } else {
                NoRayTarget();
            }

            if (pointer) {
                if (pointerTarget != Vector3.zero) {
                    //Update Pointer
                    pointer.gameObject.SetActive(true);
                    pointer.position = pointerTarget;
                }
                else {
                    //Hide Pointer
                    pointer.gameObject.SetActive(false);
                }
            }
        }
        #endregion;

        #region Custom Methods
        //Standard input stuff unrelated to raycasting
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.L)) ToggleLightLock();
            if (Input.GetKeyDown(KeyCode.F)) shipManager.CycleEditMode();
            if (Input.GetKeyDown(KeyCode.Alpha1)) shipManager.editMode = (EditMode)0;
            if (Input.GetKeyDown(KeyCode.Alpha2)) shipManager.editMode = (EditMode)1;
            if (Input.GetKeyDown(KeyCode.Alpha3)) shipManager.editMode = (EditMode)2;
        }

        //Raycast has a hit
        private void HandleRayTarget(RaycastHit hit)
        {
            currentShape = hit.transform.GetComponent<DynamicShape>();
            if (currentShape) {
                HandleShape(hit, currentShape);
            } else {
                demoBlock.gameObject.SetActive(false);
            }
        }

        //No raycast target
        private void NoRayTarget()
        {
            if (refVertex != null) {
                refVertex = null;
                SetPointerMaterial(pointerMainMaterial);
            }

            CanelBuildMode();
            CancelDeformMode();
        }

        //Currently selected shape
        private void HandleShape(RaycastHit hit, DynamicShape shape)
        {
            switch (shipManager.editMode) {
                case EditMode.build:
                    CancelDeformMode();
                    CancelSelectMode();
                    demoBlock.gameObject.SetActive(true);
                    Vector3 hitPoint = buildManager.Grid.GetNearestObjectWorldPosition(hit.point); //This takes the nearest cell, as a shape could be in multiple
                    demoBlock.position = hitPoint + hit.normal * buildManager.BlockScale;

                    if (Input.GetMouseButtonDown(0)) {
                        if (buildManager.Grid.GridSpaceEmpty(buildManager.Grid.WorldToGridPos(demoBlock.position))) {
                            buildManager.GenerateCube(demoBlock.position, buildManager.DefaultScale);
                        }
                    }

                    if (Input.GetMouseButtonDown(1)) {
                        //Destroy shape
                        buildManager.DestroyShape(shape);
                    }
                    break;
                case EditMode.select:
                    CanelBuildMode();
                    CancelDeformMode();

                    if (Input.GetMouseButtonDown(0)) {
                        SelectShape(shape);
                    }

                    if (Input.GetMouseButtonDown(1)) {
                        if (selectedShapes.Count > 1) {
                            DynamicShape newShape = buildManager.CombineCubes(selectedShapes);
                            if (newShape) {
                                CancelSelectMode();
                                SelectShape(newShape);
                            }
                                
                        }
                        else if (selectedShapes.Count == 1) {
                            buildManager.SplitCubes(selectedShapes[0]);
                            CancelSelectMode();
                        }
                    }

                    break;
                case EditMode.deform:
                    CanelBuildMode();
                    CancelSelectMode();
                    Vector3 surfaceHit = (hit.point - shape.transform.position) / buildManager.BlockScale * 2;
                    surfaceHit = new Vector3(Mathf.RoundToInt(surfaceHit.x), Mathf.RoundToInt(surfaceHit.y), Mathf.RoundToInt(surfaceHit.z));
                    surfaceHit = surfaceHit * buildManager.BlockScale / 2;
                    pointerTarget = shape.transform.position + surfaceHit;

                    if (Input.GetMouseButtonDown(1)) {
                        shape.BuildBasicCube(Vector3.zero, buildManager.BlockScale); //BUG!!!! Does not recall scale
                    }

                    if (Input.GetMouseButtonDown(0)) {
                        SetPointerMaterial(selectionMaterial);
                        refVertex = shape.GetNearestVertex(surfaceHit);
                        if (refVertex.IsrealVertex()) {
                            selectedDeformShape = shape;
                            selectedDeformShape.SetMaterial(demoMaterial);
                        }
                    }

                    if (Input.GetMouseButtonUp(0)) {
                        SetPointerMaterial(pointerMainMaterial);
                        if (refVertex != null) {
                            if (refVertex.IsrealVertex()) {
                                refVertex.point = surfaceHit;
                            }
                        }
                        CancelDeformMode();
                    }
                    break;
                default:
                    break;
            }
        }

        private void CancelDeformMode()
        {
            if (selectedDeformShape) {
                selectedDeformShape.GenerateShape();
                selectedDeformShape.SetMaterial(buildManager.StandardMaterial);
                selectedDeformShape = null;
                refVertex = null;
            }
        }
        private void CanelBuildMode()
        {
            if (demoBlock) {
                demoBlock.gameObject.SetActive(false);
            }

        }
        private void CancelSelectMode()
        {
            if (selectedShapes.Count > 0) {
                foreach (var item in selectedShapes) {
                    item.SetMaterial(buildManager.StandardMaterial);
                }
                selectedShapes = new List<DynamicShape>();
            }
        }

        private void SelectShape(DynamicShape shape)
        {
            if (selectedShapes.Contains(shape)) {
                selectedShapes.Remove(shape);
                shape.SetMaterial(buildManager.StandardMaterial);
            } else {
                selectedShapes.Add(shape);
                shape.SetMaterial(selectionMaterial);
            }
        }

        /// <summary>
        /// Toggles between the light being static and attached to camera.
        /// </summary>
        private void ToggleLightLock()
        {
            if (mainCamera && mainLight) {
                //Apply
                if (cameraHasLight) {
                    mainLight.SetParent(mainCamera.transform);
                    mainLight.transform.localPosition = new Vector3(0, 0, 0);
                }
                else {
                    mainLight.transform.parent = null;
                }

                //Toggle
                cameraHasLight = !cameraHasLight;
            }
        }

        private void SetPointerMaterial(Material mat)
        {
            if (pointer && mat) {
                pointer.GetComponent<Renderer>().material = mat;
            }
        }
        #endregion
    }
}
