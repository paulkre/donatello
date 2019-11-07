using UnityEngine;

using VRSculpting.Tools;

namespace VRSculpting.UI.ToolSelectionUI {

	[RequireComponent(typeof(Renderer))]
	public class OptionBehaviour : MonoBehaviour {

		private float alpha = 0;
		private float targetAlpha = 0;

		public float Alpha {
			set {
				targetAlpha = value;
			}
		}

		public bool Selected {
			set {
				GetComponent<Renderer>().material.color = new Color(1, 1, 1, value ? 1 : .5f);
			}
		}

		public Texture Texture {
			set {
				GetComponent<Renderer>().material.mainTexture = value;
			}
		}

		public ToolType tool;

		private void Update() {
			alpha += (targetAlpha - alpha) * .15f;
			GetComponent<Renderer>().material.color = new Color(1, 1, 1, alpha);
		}

	}

}
