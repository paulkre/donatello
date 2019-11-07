using System.Collections.Generic;
using UnityEngine;

namespace VRSculpting.Sculptor {
	using Tools;
	using SculptMesh;
	using Settings;

	public abstract class SculptorBehaviour : MonoBehaviour {

		[SerializeField]
		private List<UI.UI> uiComponents;

		protected Menu Menu { get; private set; }

		private static bool symmetry = true;

		private ToolCollection mainColl;

		private SculptState state;

		protected MeshWrapperBehaviour MeshWrapper { get; private set; }

		public virtual void Init(ISculptMesh sculptMesh) {
			MeshWrapper = sculptMesh.Wrapper;

			Menu = new Menu(mainColl, ToolType.Standard);

			mainColl = new ToolCollection(sculptMesh, Menu);

			Menu.OnToolChange += HandleToolChange;

			uiComponents.ForEach(ui => ui.Init(Menu));
		}

		private void HandleToolChange(ToolType tool) {
			MeshWrapper.SculptMesh.Deformer.Unmask();
		}

		public void Sculpt() {
			state = GetState(state);

			var mat = MeshWrapper.Material;
			mat.SetVector($"_BrushPos", state.position);
			mat.SetFloat($"_BrushRadius", Menu.ToolSize.Value / 2);
			mat.SetFloat($"_BrushHardness", Menu.ToolHardness.Value);

			if (state.drawing)
				mainColl[Menu.CurrentTool].Use(state);
			else if (state.drawingUp)
				MeshWrapper.SculptMesh.Deformer.Unmask();
		}

		protected abstract SculptState GetState(SculptState prev);
	}
}
