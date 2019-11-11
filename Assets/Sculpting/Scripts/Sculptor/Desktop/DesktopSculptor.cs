using UnityEngine;

namespace VRSculpting.Sculptor.Desktop {
	using Tools;

	public class DesktopSculptor : SculptorBehaviour {

		[SerializeField]
		public DesktopCamRig camRigPrefab;

		private DesktopCamRig camRig;

		private static float parameterModStrength = .1f;

		private Vector3 activePosition;
		
		private void Awake() {
			if (camRigPrefab == null) Debug.LogError(@"Camera rig reference not found on ""DesktopSculptor""");
			camRig = Instantiate(camRigPrefab);
		}

		protected override SculptState GetState(SculptState prev) {
			if (Input.GetKeyDown("1")) Menu.CurrentTool = ToolType.Standard;
			if (Input.GetKeyDown("2")) Menu.CurrentTool = ToolType.Move;
			if (Input.GetKeyDown("3")) Menu.CurrentTool = ToolType.Smooth;

			if (Input.GetKeyDown("w")) Menu.ToolSize.Value += parameterModStrength;
			if (Input.GetKeyDown("q")) Menu.ToolSize.Value -= parameterModStrength;
			
			if (Input.GetKeyDown("s")) Menu.ToolHardness.Value += parameterModStrength;
			if (Input.GetKeyDown("a")) {
				var p = Menu.ToolHardness;
				p.Value = Mathf.Max(.1f, p.Value - parameterModStrength);
			}

			bool isNavigating = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);

			var drawing = !isNavigating && Input.GetMouseButton(0);
			var drawingDown = !isNavigating && Input.GetMouseButtonDown(0);
			var drawingUp = !isNavigating && Input.GetMouseButtonUp(0);
			var brushPosition = GetBrushPosition(drawing, drawingDown, drawingUp);

			return new SculptState {
				position = brushPosition,
				strength = 1f,

				drawing = drawing,
				drawingDown = drawingDown,
				drawingUp = drawingUp,
				drawingInverted = !isNavigating && Input.GetKey(KeyCode.LeftControl),
			};
		}

		private Vector3 GetBrushPosition(bool isDrawing, bool isDrawingDown, bool isDrawingUp) {
			Ray ray = camRig.Cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			bool intersected = Physics.Raycast(ray, out hit, Mathf.Infinity);

			if (!isDrawing && !isDrawingUp && intersected) return hit.point;
			
			Vector3 camPivot = camRig.transform.position;
			Vector3 camPos = camRig.Cam.transform.position;
			Vector3 planePos;

			if (isDrawingDown)
				planePos = activePosition = intersected
					? Vector3.Project(hit.point - camPos, camPivot - camPos) + camPos
					: camPivot;
			else if (isDrawing || isDrawingUp) planePos = activePosition;
			else planePos = camPivot;

			Plane plane = new Plane(camPos - planePos, planePos);
			float enter = 0.0f;
			plane.Raycast(ray, out enter);

			return ray.GetPoint(enter);
		}
	}
}
