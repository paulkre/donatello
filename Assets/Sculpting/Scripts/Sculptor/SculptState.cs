using UnityEngine;

namespace VRSculpting.Sculptor {

	public struct SculptState {
		public Vector3 position;
		public float strength;

		public bool drawing;
		public bool drawingDown;
		public bool drawingUp;
		public bool drawingInverted;
	}

}
