using UnityEngine;

namespace VRSculpting.Sculptor {
	using Settings;

	public struct SculptState {
		public Vector3 position;
		public float strength;

		public bool drawing;
		public bool drawingDown;
		public bool drawingUp;
		public bool drawingInverted;

		public Matrix4x4 worldToLocal;

		public MenuState menuState;
	}

}
