using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fishy.DataTypes;

namespace Fishy.GridsOld {
    [Serializable]
    public class Grid3D<T> where T : IGridObject {

        #region Variables
        private Vector3 gridCenter; //World space center of grid
        private int size; //The width height and length of the grid
        private int area; 
        private float scale;
        //private T[,,] gridArray;
        private Dictionary<Short3, T> objectDictionary = new Dictionary<Short3, T>();
        private Dictionary<T, Short3[]> vectorDictionary = new Dictionary<T, Short3[]>();
        #endregion;

        #region Properties
        public Vector3 GridCenter { get { return gridCenter; } }
        public int Size { get { return size; } }
        public float Scale { get { return scale; } set { scale = value; } }
        public float Area { get { return area; } }
        #endregion;

        #region Constructor
        public Grid3D(Vector3 gridCenter, int gridSize, float scale)
        {
            this.gridCenter = gridCenter;
            this.size = gridSize;
            this.scale = scale;

            int ags = (gridSize * 2) + 1;
            //gridArray = new T[ags, ags, ags];

            area = (int)Mathf.Pow(gridSize * 2f, 3f);
        }
        #endregion

        #region Private Methods
        private Vector3 SanitizeVector(Vector3 v)
        {
            return new Vector3(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if the world position is near a free grid vector
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public bool WorldSpaceEmpty(Vector3 worldPosition)
        {
            return GridSpaceEmpty(WorldToGridPos(worldPosition));
        }

        /// <summary>
        /// Checks if the grid position is near a free grid vector
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public bool GridSpaceEmpty(Short3 gridPosition)
        {
            return (!objectDictionary.ContainsKey(gridPosition) && GridPositionInGrid(gridPosition));
        }

        /// <summary>
        /// Adds an object to the grid. Returns true if successful 
        /// </summary>
        /// <param name="gridObject"></param>
        /// <returns></returns>
        public bool AddObject(T gridObject)
        {
            if (!WorldSpaceEmpty(gridObject.WorldPosition)) return false;

            Short3 gridPos = WorldToGridPos(gridObject.WorldPosition);
            Short3[] GridArray = new Short3[1];
            GridArray[0] = WorldToGridPos(gridObject.WorldPosition);

            objectDictionary.Add(gridPos, gridObject);
            vectorDictionary.Add(gridObject, GridArray);
            return true;
        }

        /// <summary>
        /// DEPREICATED
        /// Adds object to grid at passed world position
        /// </summary>
        /// <param name="gridObject"></param>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public bool AddObjectAtWorldPosition(T gridObject, Vector3 worldPosition)
        {
            if (!WorldSpaceEmpty(worldPosition)) return false;

            Short3 gridPos = WorldToGridPos(worldPosition);
            Short3[] GridArray = new Short3[1];
            GridArray[0] = WorldToGridPos(worldPosition);

            objectDictionary.Add(gridPos, gridObject);
            vectorDictionary.Add(gridObject, GridArray);
            return true;
        }

        /// <summary>
        /// DEPREICATED
        /// Adds object to grid at passed grid space
        /// </summary>
        /// <param name="gridObject"></param>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public bool AddObjectAtGrid(T gridObject, Vector3 gridPosition)
        {
            if (!GridSpaceEmpty(gridPosition)) return false;

            Short3[] GridArray = new Short3[1];
            GridArray[0] = gridPosition;

            objectDictionary.Add(gridPosition, gridObject);
            vectorDictionary.Add(gridObject, GridArray);
            return true;
        }

        /// <summary>
        /// Adds object to grid at passed grid positions. It will take up multiple spaces, hence we are dealing with grid space from the offset.
        /// </summary>
        /// <param name="gridObject"></param>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public bool AddObject(T gridObject, Short3[] gridPositions)
        {
            int spacesFree = 0;

            for (int i = 0; i < gridPositions.Length; i++) {
                if (GridSpaceEmpty(gridPositions[i])) spacesFree++;
            }

            if (spacesFree == gridPositions.Length) {
                //All spaces are free- add

                for (int i = 0; i < gridPositions.Length; i++) {
                    if (!objectDictionary.ContainsKey(gridPositions[i])) {
                        objectDictionary.Add(gridPositions[i], gridObject);
                    } else {
                        Debug.LogWarning("cant add grid refference " + gridPositions[i] + ": already in dictionary");
                    }
                }

                vectorDictionary.Add(gridObject, gridPositions);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes all references to the passed object in grid
        /// </summary>
        /// <param name="gridObject">grid object</param>
        /// <returns></returns>
        public bool RemoveObject(T gridObject)
        {
            Short3[] worldArray;
            if (vectorDictionary.TryGetValue(gridObject, out worldArray)) {
                for (int i = 0; i < worldArray.Length; i++) {
                    objectDictionary.Remove(worldArray[i]);
                }
                return vectorDictionary.Remove(gridObject);
            }
            return false;
        }

        /// <summary>
        /// Removes all references to the passed object in grid
        /// </summary>
        /// <param name="gridPosition">Grid position</param>
        /// <returns></returns>
        public bool RemoveObject(Vector3 gridPosition)
        {
            T gridObj;
            if (objectDictionary.TryGetValue(gridPosition, out gridObj)) {
                objectDictionary.Remove(gridPosition);
                return vectorDictionary.Remove(gridObj);
            }
            return false;
        }

        public Short3[] GetObjectGridPosition(T gridObject)
        {
            Short3[] vectorOut;
            vectorDictionary.TryGetValue(gridObject, out vectorOut);
            return vectorOut;
        }

        /// <summary>
        /// remove from grid at world position
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public bool ClearWorldPosition(Vector3 worldPosition)
        {
            return objectDictionary.Remove(WorldToGridPos(worldPosition));
        }

        /// <summary>
        /// Remove from grid at grid space
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public bool ClearGridPosition(Vector3 gridPosition)
        {
            return objectDictionary.Remove(gridPosition);
        }

        /// <summary>
        /// Clears all objects from the grid
        /// </summary>
        public void ClearGrid()
        {
            objectDictionary.Clear();
        }

        /// <summary>
        /// Checks if the given world vector lies within the range of the grid
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public bool WorldPositionInGrid(Vector3 worldPosition)
        {
            return GridPositionInGrid(WorldToGridPos(worldPosition));
        }

        /// <summary>
        /// Checks if the given grid vector lies within the range of the grid
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public bool GridPositionInGrid(Vector3 gridPosition)
        {
            Vector3 v = SanitizeVector(gridPosition);
            return (
                v.x >= -size && v.x < size &&
                v.y >= -size && v.y < size &&
                v.z >= -size && v.z < size
            );
        }

        /// <summary>
        /// Converts a grid position into world
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public Vector3 GridToWorldPos(Short3 gridPosition)
        {
            return gridCenter + ((Vector3)gridPosition * scale);
        }

        /// <summary>
        /// Converts a world position into a grid
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public Short3 WorldToGridPos(Vector3 worldPosition)
        {
            return SanitizeVector((worldPosition - gridCenter) / scale);
        }

        /// <summary>
        /// Returns a world vector that aligns with the grid
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public Vector3 SnapToGrid(Vector3 worldPosition)
        {
            return GridToWorldPos(WorldToGridPos(worldPosition));
        }

        /// <summary>
        /// Finds the nearest world point that is occupied in grid
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns>Nearest point in world space</returns>
        public Vector3 GetNearestObjectWorldPosition(Vector3 worldPosition)
        {
            return GridToWorldPos(GetNearestObjectGridPosition(worldPosition));
        }

        /// <summary>
        /// Finds the nearest grid space that is occupied
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns>Nearest point in world space</returns>
        public Vector3 GetNearestObjectGridPosition(Vector3 worldPosition)
        {
            float bestDist = float.MaxValue;
            Vector3 bestVector = Vector3.zero;

            foreach (var item in objectDictionary) {
                float dist = (GridToWorldPos(item.Key) - worldPosition).sqrMagnitude;
                if (dist < bestDist) {
                    bestVector = item.Key;
                    bestDist = dist;
                }
            }

            return bestVector;
        }

        /// <summary>
        /// Finds the nearest object to the given world position
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public T GetNearestObject(Vector3 worldPosition)
        {
            T objectOut;
            objectDictionary.TryGetValue(GetNearestObjectGridPosition(worldPosition), out objectOut);
            return objectOut;
        }

        /// <summary>
        /// Finds the object at grid position
        /// </summary>
        /// <param name="gridPosition"></param>
        /// <returns></returns>
        public T GetObjectAtGrid(Short3 gridPosition)
        {
            T objectOut;
            objectDictionary.TryGetValue(SanitizeVector(gridPosition), out objectOut);
            return objectOut;
        }

        /// <summary>
        /// Returns a float between 1 and 0, denoting how full this area is. Value of 1 means every cell is occupied
        /// </summary>
        /// <param name="start">Grid space</param>
        /// <param name="end">Grid space</param>
        /// <param name="list">Optional list of grid points to check instead of all</param>
        /// <returns></returns>
        public float GetAreaFill(Short3 start, Short3 end, List<Vector3> list = null)
        {
            float fill = 0f;

            start = SanitizeVector(start);
            end = SanitizeVector(end);

            float fillMax = (1 + end.x - start.x) * (1 + end.y - start.y) * (1 + end.z - start.z);

            for (int x = (int)start.x; x <= end.x; x++) {
                for (int y = (int)start.y; y <= end.y; y++) {
                    for (int z = (int)start.z; z <= end.z; z++) {
                        Vector3 v = new Vector3(x, y, z);
                        
                        if (list != null) {
                            if (list.Contains(v)) {
                                fill++;
                            }
                        } else {
                            if (objectDictionary.ContainsKey(v)) {
                                fill++;
                            }
                        }
                        
                    }
                }
            }
            return fill / fillMax;
        }

        /// <summary>
        /// Returns a float between 1 and 0, denoting how full this area is. Value of 1 means every cell is occupied.
        /// This will only check against provided gridPoints, so will not count as full if cells are full but not in the list.
        /// </summary>
        /// <param name="gridPoints">List of all grids to check</param>
        /// <returns></returns>
        public float GetAreaFill(List<Vector3> gridPoints)
        {
            GridData data = new GridData(gridPoints);
            return GetAreaFill(data.min, data.max, gridPoints);
        }

        /// <summary>
        /// Prints a list of all objects in the grid
        /// </summary>
        public void PrintObjectsToLog()
        {
            foreach (var obj in vectorDictionary) {
                Debug.Log(obj.Key.ToString() + ": " + obj.Value[0]);
            }
        }

        /// <summary>
        /// Returns all grid objects
        /// </summary>
        /// <returns></returns>
        public T[] GetGridObjects()
        {
            T[] array = new T[vectorDictionary.Count];

            int i = 0;
            foreach (var obj in vectorDictionary) {
                array[i] = obj.Key;
                i++;
            }

            return array;
        }
        #endregion
    }
}
