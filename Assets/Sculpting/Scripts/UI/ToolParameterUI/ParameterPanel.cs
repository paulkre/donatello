using UnityEngine;
using UnityEngine.UI;

namespace VRSculpting.UI.ToolParameterUI {
	using Settings;

	public class ParameterPanel : MonoBehaviour {

		private static float labelActiveDuration = 2f;
		private static float indiActiveDuration = .2f;

		[SerializeField]
		private Text label;

		[SerializeField]
		private Text changeIndicator;

		public Parameter Param { get; private set; }

		public bool Active { get; set; }

		private float lastChange;

		private void OnParameterChange(Parameter param, float lastValue) {
			changeIndicator.text = param.Value >= lastValue ? "▲" : "▼";
			lastChange = Time.time;
		}

		private float labelAlpha;
		private float indiAlpha;

		public void Init(Parameter param) {
			Param = param;

			labelAlpha = 0;

			lastChange = Mathf.NegativeInfinity;

			label.text = param.Label;

			param.OnChange += OnParameterChange;
			OnParameterChange(param, param.Value);
		}

		private void Update() {
			float labelTargetAlpha = Active && Time.time - lastChange <= labelActiveDuration ? 1f : 0f;
			labelAlpha += (labelTargetAlpha - labelAlpha) * .15f;

			float indiTargetAlpha = Active && Time.time - lastChange <= indiActiveDuration ? 1f : 0f;
			indiAlpha += (indiTargetAlpha - indiAlpha) * .15f;

			label.color = new Color(1, 1, 1, labelAlpha);
			changeIndicator.color = new Color(1, 1, 1, indiAlpha);
		}

	}

}
