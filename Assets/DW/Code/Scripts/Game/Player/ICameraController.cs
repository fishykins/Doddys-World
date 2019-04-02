using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW
{
    public interface ICameraController
    {
        void AssignCamera(Transform camera);
        void UpdateCamera();
    }
}