using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fishy.DataTypes;

namespace Fishy.Grids {
	public interface IGrid<T>
    {
        Volume Volume { get; }

        bool AddObject(T gridObject, Volume volume);
        bool RemoveObject(T gridObject);
        bool RemoveObject(Short3 gridPosition);
        bool RemoveObjects(Volume volume);
        bool RemoveObjects();
        bool ObjectInGrid(T gridObject);
        bool ObjectInGrid(T gridObject, out Volume volume);
        bool GridSpaceEmpty(Short3 gridPosition);
        bool GridSpaceEmpty(Volume volume);

        T GetNearestObject(Vector3 gridPosition); //Uses Vector3 for absolute prescision
        T GetObject(Short3 gridPosition);
        List<T> GetObjects(Volume volume);
        List<T> GetObjects();

        Short3 GetNearestEmptyPosition(T gridObject);
        Short3 GetNearestEmptyPosition(Vector3 gridPosition); //Uses Vector3 for absolute prescision
        Short3 GetNearestOccupiedPosition(Vector3 gridPosition, short maxRange = 8); //Uses Vector3 for absolute prescision

        Volume GetVolume(T gridObject);
        Volume GetVolume(Short3 gridPosition);
        List<Volume> GetAdjoiningVolumes(Volume volume);
    }
}
