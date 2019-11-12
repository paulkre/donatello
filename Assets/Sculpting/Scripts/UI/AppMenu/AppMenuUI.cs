using UnityEngine;

namespace VRSculpting.UI.AppMenu {

	[RequireComponent(typeof(Canvas))]
	public class AppMenuUI : UI {

		private float viewDistance = 1f;

		public Transform rightController;

		public Button exportButtonReference;

		public Pointer pointerPrefab;

		private Pointer pointer;

		private new bool enabled;

		private bool Enabled {
			set {
				GetComponent<Canvas>().enabled = value;
				pointer.Enabled = value;
				enabled = value;
			}
		}

		public override void Init(Settings.Menu menu) {
			pointer = Instantiate(pointerPrefab, rightController, false);

			Enabled = menu.AppMenuEnabled.Value;
			menu.AppMenuEnabled.OnChange += (value) => {
				Enabled = value;
			};
		}

		private void Update() {
			if (!enabled) return;

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
