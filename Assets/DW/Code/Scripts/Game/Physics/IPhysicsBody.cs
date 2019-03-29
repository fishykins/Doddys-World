using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW.Physics {
	public interface IPhysicsBody {
        void Initialize(SceneInstance scene);
        SceneInstance Scene { get; }
        Transform Transform { get; }
        IInput Controller { get; set; }
	}
}
