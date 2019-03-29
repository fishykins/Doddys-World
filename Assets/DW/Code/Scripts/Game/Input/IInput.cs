using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW {
	public interface IInput {

		float XAxis { get; }
		float YAxis { get; }
        float ZAxis { get; }

        float XRot { get; }
        float YRot { get; }
        float ZRot { get; }
	}
}
