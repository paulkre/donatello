using System;
using UnityEngine;

namespace VRSculpting.Settings {
	using Tools;

	public class Menu {

		public Menu(ToolType initialTool) {
			currentTool = initialTool;

			Parameters = new Parameter[] {
				new Parameter("size", "Size", .25f),
				new Parameter("hardness", "Hardness", 2f)
			};

			selectedParameterId = 0;

			ExportAction = new Action();
			AppMenuEnabled = new Switch();
			DoAction = new Action();

			ExportAction.OnDone += () => {
				Debug.Log("EXPORTING");
			};
		}

		public MenuState GetState() {
			return new MenuState {
				tool = CurrentTool,
				toolSize = ToolSize.Value,
				toolHardness = ToolHardness.Value,
				appMenuEnabled = AppMenuEnabled.Value
			};
		}

		#region tool-selection

		public delegate void OnToolChangeHandler(ToolType tool);

		public event OnToolChangeHandler OnToolChange;

		private ToolType currentTool;
		public virtual ToolType CurrentTool {
			get { return currentTool; }
			set {
				if (currentTool == value) return;

				currentTool = value;
				OnToolChange?.Invoke(currentTool);
			}
		}

		public void OffsetSelection(int offset) {
			var arr = (ToolType[])Enum.GetValues(typeof(ToolType));
			int i = Array.IndexOf(arr, CurrentTool) + offset;
			i = Mathf.Clamp(i, 0, arr.Length - 1);
			CurrentTool = arr[i];
		}

		#endregion

		#region parameters

		public Parameter[] Parameters { get; private set; }

		public delegate void OnParameterChangeHandler(Parameter parameter);
		public event OnParameterChangeHandler OnParameterChange;

		private int selectedParameterId;
		public int SelectedParameterId {
			get { return selectedParameterId; }
			set {
				if (value == selectedParameterId) return;

				if (value >= Parameters.Length)
					value %= Parameters.Length;
				else
					while (value < 0) value += Parameters.Length;

				selectedParameterId = value;

				OnParameterChange?.Invoke(SelectedParameter);
			}
		}

		public Parameter SelectedParameter { get { return Parameters[selectedParameterId]; } }

		public Parameter ToolSize { get { return Parameters[0]; } }
		public Parameter ToolHardness { get { return Parameters[1]; } }

		#endregion

		#region settings

		public Switch AppMenuEnabled { get; private set; }

		public Action ExportAction { get; private set; }

		public Action DoAction { get; private set; }

		#endregion

	}

}
