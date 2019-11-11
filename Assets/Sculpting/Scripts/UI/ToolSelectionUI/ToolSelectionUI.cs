using System.Collections.Generic;
using UnityEngine;

namespace VRSculpting.UI.ToolSelectionUI {
	using Settings;
	using Tools;

	public class ToolSelectionUI : UI {

		private static float optionSize = .1f;
		private static float optionGap = .02f;

		[SerializeField]
		public OptionBehaviour optionPrefab;

		[SerializeField]
		public List<OptionProps> optionProps;

		private Transform optionRoot;
		private OptionBehaviour[] options;

		private int selectedId;

		private void Update() {
			float current = optionRoot.localPosition.x;
			float target = - selectedId * (optionSize + optionGap);
			optionRoot.localPosition = (current + (target - current) * .15f) * Vector3.right;
			optionRoot.localRotation = Quaternion.Euler(Vector3.zero);
		}

		public override void Init(Menu menu) {
			CreateOptions();

			menu.OnToolChange += OnToolChange;
			OnToolChange(menu.CurrentTool);
		}

		private void OnToolChange(ToolType currentTool) {
			selectedId = optionProps.FindIndex((props) => props.tool == currentTool);

			for (int i = 0; i < options.Length; i++)
				options[i].Alpha = i == selectedId ? 1 : .15f;
		}

		private void CreateOptions() {
			options = new OptionBehaviour[optionProps.Count];

			optionRoot = (new GameObject()).transform;
			optionRoot.parent = transform;
			optionRoot.localPosition = Vector3.zero;
			optionRoot.name = "Options";

			for (int i = 0; i < optionProps.Count; i++) {
				var props = optionProps[i];
				var option = Instantiate(optionPrefab);

				option.tool = props.tool;
				option.name = props.label;
				option.Texture = props.icon;
				option.transform.parent = optionRoot;
				option.transform.localPosition = i * (optionSize + optionGap) * Vector3.right;
				option.transform.localScale = optionSize * Vector3.one;
				option.Alpha = 0;

				options[i] = option;
			}
		}

	}

}
