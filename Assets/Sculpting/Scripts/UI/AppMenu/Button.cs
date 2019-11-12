using UnityEngine;
using UnityEngine.UI;

namespace VRSculpting.UI.AppMenu {

	public class Button : MonoBehaviour {

		public string label;

		public Text labelReference;

		private void Awake() {
			labelReference.text = label;

			var coll = gameObject.AddComponent<BoxCollider>();

			float width = (transform.parent.parent as RectTransform).sizeDelta.x;
			float height = (transform as RectTransform).sizeDelta.y;

			coll.size = new Vector3(width, height);
		}

	}

}
