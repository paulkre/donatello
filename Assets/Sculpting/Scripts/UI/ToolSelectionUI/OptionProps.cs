using UnityEngine;

namespace VRSculpting.UI.ToolSelectionUI {
	using Tools;

	[System.Serializable]
	public struct OptionProps {
		[SerializeField]
		public string label;

		[SerializeField]
		public ToolType tool;

		[SerializeField]
		public Texture icon;
	}

}
