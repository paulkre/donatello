using UnityEngine;
using UnityEngine.XR;

namespace VRSculpting.Sculptor.Odyssey {
	using SculptMesh;

	public class OdysseySculptor : SculptorBehaviour {

		[SerializeField]
		private Transform leftController;

		[SerializeField]
		private Transform rightController;

		private static Vector3 effectPointOffset = new Vector3(.015f, -.05f, .12f);

		private bool lastDrawingState;

		private TransformInputManager transformInputManager;
		private ToolInputManager toolInputManager;
		private ParameterInputManager parameterInputManager;

		public override void Init(ISculptMesh mesh) {
			base.Init(mesh);

			transformInputManager = new TransformInputManager(MeshWrapper);
			toolInputManager = new ToolInputManager(Menu);
			parameterInputManager = new ParameterInputManager(Menu);
		}

		protected override SculptState GetState(SculptState prev) {
			ManageInput();

			float strength = Input.GetAxis("Odyssey Trigger Squeeze Right");
			bool drawing = strength > 0f;

			var state = new SculptState {
				position = rightController.TransformPoint(effectPointOffset),
				strength = strength,

				drawing = drawing,
				drawingDown = drawing && !lastDrawingState,
				drawingUp = !drawing && lastDrawingState,
				drawingInverted = Input.GetButton("Odyssey Trigger Left"),
			};

			lastDrawingState = drawing;

			return state;
		}

		private void ManageInput() {
			UpdateControllerTransforms(leftController, XRNode.LeftHand);
			UpdateControllerTransforms(rightController, XRNode.RightHand);

			string gripRightInput = "Odyssey Grip Press Right";
			string gripLeftInput = "Odyssey Grip Press Left";
			transformInputManager.ManageInput(
				rightController.position,
				leftController.position,
				Input.GetButton(gripRightInput),
				Input.GetButton(gripLeftInput),
				Input.GetButtonDown(gripRightInput),
				Input.GetButtonDown(gripLeftInput),
				Input.GetButtonUp(gripRightInput),
				Input.GetButtonUp(gripLeftInput)
			);

			toolInputManager.ManageInput(Input.GetAxis("Odyssey Thumbstick Horizontal Left"));

			parameterInputManager.ManageInput(Input.GetAxis("Odyssey Thumbstick Horizontal Right"));
		}

		private static void UpdateControllerTransforms(Transform controller, XRNode node) {
			controller.position = InputTracking.GetLocalPosition(node);
			controller.rotation = InputTracking.GetLocalRotation(node);
		}

	}
}
