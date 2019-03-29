using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fishy.DataTypes;

namespace DW.Building {
	public static class BuildingUtil {
        public static Vector3 GridToWorld(Vector3 gridPosition, Vector3 gridWorldAnchor, float gridScale)
        {
            return (gridPosition * gridScale) + gridWorldAnchor;
        }

        public static Vector3 WorldToGrid(Vector3 worldPosition, Vector3 gridWorldAnchor, float gridScale)
        {
            return (worldPosition - gridWorldAnchor) / gridScale;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The name to show in inspeector</param>
        /// <param name="parent">parent transform to child to</param>
        /// <param name="material">material to set</param>
        /// <param name="mesh">mesh to set</param>
        /// <param name="hasCollider">if true, will add a collider</param>
        /// <returns></returns>
        public static GameObject GenerateMeshObject(string name = "Mesh Object", Transform parent = null, Material material = null, Mesh mesh = null, bool hasCollider = false)
        {
            GameObject meshObject = new GameObject(name);
            if (parent != null) {
                meshObject.transform.SetParent(parent);
            }
            
            MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();

            if (material != null) {
                meshObject.GetComponent<Renderer>().material = material;
            }

            if (mesh != null) {
                if (meshFilter != null) {
                    meshFilter.sharedMesh = mesh;
                }

                if (hasCollider) {
                    MeshCollider meshCollider = meshObject.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = mesh;
                }
            }

            return meshObject;
        }
    }
}
