using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW.Physics {
	public interface IGravityBody {
        Vector3 GetGravitationalForce(Vector3 position, float mass);
	}
}
