using UnityEngine;

namespace VRSculpting.UI.ToolParameterUI {
	using Settings;

	public class ToolParameterUI : UI {

		[SerializeField]
		private ParameterPanel parameterPanelPrefab;

		private ParameterPanel[] panels;

		private int selectedId;
		private float panelHeight;
		private float panelOffset;

		public override void Init(Menu menu) {
			panels = CreatePanels(menu.Parameters);

			menu.OnParameterChange += OnParameterChange;
			OnParameterChange(menu.SelectedParameter);

			selectedId = 0;
			panelHeight = (panels[0].transform as RectTransform).rect.height;
			panelOffset = 0f;
		}

		private void OnParameterChange(Parameter param) {
			for (int i = 0; i < panels.Length; i++) {
				var panel = panels[i];
				bool isActive = panel.Param == param;
				panel.Active = isActive;
				if (isActive) selectedId = i;
			}
		}

		private ParameterPanel[] CreatePanels(Parameter[] parameters) {
			var panels = new ParameterPanel[parameters.Length];
			for (int i = 0; i < parameters.Length; i++) {
				var param = parameters[i];
				var panel = Instantiate(parameterPanelPrefab, transform, false);
				panel.Init(param);
				panels[i] = panel;
			}
			return panels;
		}

		private void Update() {

		}

	}

}
