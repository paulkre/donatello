using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSculpting.Sculptor.Desktop {
	public class DesktopCamRig : MonoBehaviour {
		[SerializeField]
		private Camera cam;

		[SerializeField]
		private Transform vertPivot;

		[SerializeField]
		private float zoomStrength = .0008f;
		[SerializeField]
		private float orbitStrength = .2f;
		[SerializeField]
		private float panStrength = .0008f;
		[SerializeField, Range(0, 10)]
		private float camDist;

		private Vector3 lastMousePos;

		public Camera Cam {
			get { return cam; }
		}

		private Vector3 MouseDelta {
			get { return Input.mousePosition - lastMousePos; }
		}

		private void Start() {
			camDist = 1f;
		}

		private void Update() {
			if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) HandleInput();
			lastMousePos = Input.mousePosition;
			cam.transform.localPosition = -Vector3.forward * camDist;
		}

		private void HandleInput() {
			if (Input.GetMouseButton(0)) {
				Vector3 delta = MouseDelta * orbitStrength;
				transform.Rotate(0, delta.x, 0);
				vertPivot.Rotate(-delta.y, 0, 0);
			}

			if (Input.GetMouseButton(1)) {
				camDist = Mathf.Max(0.0f, camDist + MouseDelta.x * zoomStrength);
			}

			if (Input.GetMouseButton(2)) {
				transform.position -= cam.transform.right * MouseDelta.x * panStrength;
				transform.position -= cam.transform.up * MouseDelta.y * panStrength;
			}
		}

	}
}
