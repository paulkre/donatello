using UnityEngine;

namespace VRSculpting.UI.AppMenu {

	public class AppMenuUI : UI {

		private float viewDistance = 1f;

		public Transform rightController;

		public Button exportButtonReference;

		public Pointer pointerPrefab;

		private Pointer pointer;

		public override void Init(Settings.Menu menu) {
			pointer = Instantiate(pointerPrefab, rightController, false);
		}

		private void Update() {
			var cam = Camera.main.transform;
			var targetPosition = cam.position + viewDistance * cam.forward;
			transform.position += (targetPosition - transform.position) * .05f;
			transform.rotation = Quaternion.LookRotation(cam.forward, cam.up);

			HandlePointer();
		}

		private void HandlePointer() {
			var trm = pointer.transform;

			RaycastHit hit;
			var intersected = Physics.Raycast(
				trm.position,
				trm.forward,
				out hit
			);

			pointer.Length = intersected
				? Vector3.Distance(trm.position, hit.point)
				: 1000;
		}

	}

}
