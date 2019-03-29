using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fishy.DataTypes;

namespace Fishy.Grids
{
    [System.Serializable]
    public class Grid3D<T> : IGrid<T>
    {
        #region Variables
        private Volume gridVolume; //The total volume of the grid
        private Dictionary<T, Volume> volumeDictionary = new Dictionary<T, Volume>();
        #endregion;

        #region Constructor
        public Grid3D(Volume volume, bool debug = false)
        {
            gridVolume = volume;
            if (debug)
                Debug.Log(gridVolume.u + " -> " + gridVolume.v);
        }
        #endregion;

        #region Private Methods
        public Volume Volume { get { return gridVolume; } }
        #endregion

        #region Public Methods
        public bool AddObject(T gridObject, Volume volume)
        {
            //volume.Santize(); //Dont let any volumes in here without cleaning it up first!

            //Debug.Log("===============================");
            //foreach (var item in GetObjects()) {
            //    Debug.Log("Object " + GetVolume(item));
            //}

            if (volumeDictionary.ContainsKey(gridObject)) return false; //Already in- cant add dupliactes
            //Debug.Log("Passed object check");
            if (!GridSpaceEmpty(volume)) {
                Debug.Log("Space check failed");
                return false; //No dice- spot is taken
            }
            //Debug.Log("Passed space Empty Check");

            volumeDictionary.Add(gridObject, volume);
            //Debug.Log("added volume " + volume);

            return true;
        }
        public bool GridSpaceEmpty(Volume volume)
        {
            if (!gridVolume.Contains(volume)) {
                //Debug.Log("Volume outside grid");
                return false; //Falls outside the grid bounds, so we can safely say it is not free
            }
            foreach (var item in volumeDictionary) {
                //Debug.Log("Checking if " + volume + " is in " + item.Value);
                if (item.Value.Intersects(volume)) {
                    //Debug.Log("Volume " + item.Value + " already occupies this space");
                    return false;
                }
            }
            return true;
        }
        public bool GridSpaceEmpty(Short3 gridPosition)
        {
            return GridSpaceEmpty(new Volume(gridPosition));
        } //Sub
        public bool ObjectInGrid(T gridObject)
        {
            return (volumeDictionary.ContainsKey(gridObject));
        }
        public bool ObjectInGrid(T gridObject, out Volume volume)
        {
            volume = GetVolume(gridObject);
            return ObjectInGrid(gridObject);
        }
        public bool RemoveObject(T gridObject)
        {
            if (!ObjectInGrid(gridObject)) return false;
            volumeDictionary.Remove(gridObject);
            return true;
        }
        public bool RemoveObjects(Volume volume)
        {
            int objectsFound = 0;
            int objectsRemoved = 0;
            foreach (var item in volumeDictionary) {
                if (volume.Contains(item.Value)) {
                    objectsFound++;
                    if (RemoveObject(item.Key)) objectsRemoved++;
                }
            }
            return (objectsRemoved > 0 && objectsRemoved == objectsFound);
        }
        public bool RemoveObjects()
        {
            return RemoveObjects(gridVolume);
        } //Sub
        public bool RemoveObject(Short3 gridPosition)
        {
            T gridObject = GetObject(gridPosition);
            return RemoveObject(gridObject);
        } //Sub

        public T GetNearestObject(Vector3 gridPosition)
        {
            return GetObject(GetNearestOccupiedPosition(gridPosition));
        }
        public T GetObject(Short3 gridPosition)
        {
            foreach (var item in volumeDictionary) {
                if (item.Value.Contains(gridPosition)) {
                    return item.Key;
                }
            }
            return default(T);
        }
        public List<T> GetObjects(Volume volume)
        {
            List<T> list = new List<T>();
            foreach (var item in volumeDictionary) {
                if (volume.Intersects(item.Value)) {
                    list.Add(item.Key);
                }
            }
            return list;
        }
        public List<T> GetObjects()
        {
            return GetObjects(gridVolume);
        } //Sub

        public Short3 GetNearestEmptyPosition(T gridObject)
        {
            throw new System.NotImplementedException();
        } 
        public Short3 GetNearestEmptyPosition(Vector3 gridPosition)
        {
            if (volumeDictionary.Count == 0) return gridPosition;


            return gridPosition;
        }  //MAKE ME!!!
        public Short3 GetNearestOccupiedPosition(Vector3 gridPosition, short maxRange = 8)
        {
            Short3 nearestPos = gridPosition;

            short range = 0;

            while(range < maxRange) {
                Vector3 point = new Vector3(range, range, range);
                Volume area = new Volume(gridPosition - point, gridPosition + point);
                List<T> objects = GetObjects(area);

                if (objects.Count > 0) {
                    //We have found some volumes, so one of these will be the closest. 
                    range = maxRange;

                    float nearestDist = float.MaxValue;

                    foreach (var item in objects) {
                        Volume volume = GetVolume(item);
                        int x = Mathf.Clamp(Mathf.RoundToInt(gridPosition.x), volume.u.x, volume.v.x);
                        int y = Mathf.Clamp(Mathf.RoundToInt(gridPosition.y), volume.u.y, volume.v.y);
                        int z = Mathf.Clamp(Mathf.RoundToInt(gridPosition.z), volume.u.z, volume.v.z);
                        Vector3 nearestVolumePos = new Vector3(x, y, z);
                        float dist = (nearestVolumePos - gridPosition).sqrMagnitude;
                        if (dist < nearestDist) {
                            nearestDist = dist;
                            nearestPos = nearestVolumePos;
                        }
                    }
                }
                range++;
            }

            return nearestPos;
        }

        public Volume GetVolume(T gridObject)
        {
            Volume volume = Volume.zero;
            volumeDictionary.TryGetValue(gridObject, out volume);
            return volume;
        }
        public Volume GetVolume(Short3 gridPosition)
        {
            foreach (var item in volumeDictionary) {
                if (item.Value.Contains(gridPosition)) {
                    return item.Value;
                }
            }
            return Volume.zero;
        }
        public List<Volume> GetAdjoiningVolumes(Volume volume)
        {
            List<Volume> volumes = new List<Volume>();

            //Top
            Short3 u = new Short3(volume.u.x, (short)(volume.v.y + 1), volume.u.z);
            Short3 v = new Short3(volume.v.x, (short)(volume.v.y + 1), volume.v.z);
            volumes.Add(new Volume(u, v));

            //Bottom
            u = new Short3(volume.u.x, (short)(volume.u.y - 1), volume.u.z);
            v = new Short3(volume.v.x, (short)(volume.u.y - 1), volume.v.z);
            volumes.Add(new Volume(u, v));

            //Left
            u = new Short3((short)(volume.u.x - 1), volume.u.y, volume.u.z);
            v = new Short3((short)(volume.u.x - 1), volume.v.y, volume.v.z);
            volumes.Add(new Volume(u, v));

            //Right
            u = new Short3((short)(volume.v.x + 1), volume.u.y, volume.u.z);
            v = new Short3((short)(volume.v.x + 1), volume.v.y, volume.v.z);
            volumes.Add(new Volume(u, v));

            //Front
            u = new Short3(volume.u.x, volume.u.y, (short)(volume.v.z + 1));
            v = new Short3(volume.v.x, volume.v.y, (short)(volume.v.z + 1));
            volumes.Add(new Volume(u, v));

            //Back
            u = new Short3(volume.u.x, volume.u.y, (short)(volume.u.z - 1));
            v = new Short3(volume.v.x, volume.v.y, (short)(volume.u.z - 1));
            volumes.Add(new Volume(u, v));

            return volumes;
        }
        #endregion
    }
}
