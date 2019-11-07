using UnityEngine;

namespace VRSculpting.Sculptor.Odyssey {
	using SculptMesh;

	public class TransformInputManager {

		private Vector3 lastTranslatePoint;
		private Vector3 lastDirection;

		private bool isScalingAndRotating;
		private float lastScaleDistance;

		private MeshWrapperBehaviour meshWrapper;

		public TransformInputManager(MeshWrapperBehaviour meshWrapper) {
			this.meshWrapper = meshWrapper;
		}

		public void ManageInput(
			Vector3 rightPoint,
			Vector3 leftPoint,
			bool gripRight,
			bool gripLeft,
			bool gripRightDown,
			bool gripLeftDown,
			bool gripRightUp,
			bool gripLeftUp
		) {
			if (isScalingAndRotating || (gripRight && gripLeft)) {
				var vec = leftPoint - rightPoint;
				var distance = vec.magnitude;

				if (gripRightDown || gripLeftDown) {
					lastScaleDistance = distance;
					lastDirection = vec;
					isScalingAndRotating = true;
				}

				var mid = (rightPoint + leftPoint) / 2;

				var scale = distance / lastScaleDistance;
				var rotation = Quaternion.FromToRotation(lastDirection, vec);

				var trm = meshWrapper.transform;

				trm.rotation *= rotation;

				trm.position -= mid;
				trm.position *= scale;
				trm.position += mid;

				trm.localScale *= scale;

				lastScaleDistance = distance;
				lastDirection = vec;

				if ((gripRightUp && !gripLeft) || (gripLeftUp && !gripRight)) {
					meshWrapper.ApplyRotation();
					isScalingAndRotating = false;
				}
			} else if (gripRight) {
				if (gripRightDown) lastTranslatePoint = rightPoint;

				meshWrapper.transform.position += rightPoint - lastTranslatePoint;
				lastTranslatePoint = rightPoint;
			}
		}

	}

}

