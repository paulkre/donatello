using System;
using UnityEngine;
using System.Linq;

namespace VRSculpting.Settings {
	using Tools;

	public class Menu {

		public Menu(ToolCollection tools, ToolType initialTool) {
			Tools = tools;
			currentTool = initialTool;

			Parameters = new Parameter[] {
				new Parameter("size", "Size", .25f),
				new Parameter("hardness", "Hardness", 2f)
			};

			selectedParameterId = 0;
		}

		#region tool-selection

		public ToolCollection Tools { get; private set; }

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

	}

}
