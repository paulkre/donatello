using System.Collections.Generic;
using UnityEngine;

namespace VRSculpting.Sculptor {
	using Tools;
	using SculptMesh;
	using SculptMesh.Modification;
	using Settings;

	public abstract class SculptorBehaviour : MonoBehaviour {

		[SerializeField]
		private List<UI.UI> uiComponents;

		protected Menu Menu { get; private set; }

		private static bool symmetry = true;

		private ToolCollection mainColl;

		private SculptState state;

		protected MeshWrapperBehaviour MeshWrapper { get; private set; }

		private Deformer deformer;

		public virtual void Init(ISculptMesh sculptMesh) {
			MeshWrapper = sculptMesh.Wrapper;

			Menu = new Menu(mainColl, ToolType.Standard);

			deformer = new Deformer(sculptMesh);

			mainColl = new ToolCollection(sculptMesh, deformer, Menu);

			Menu.OnToolChange += (ToolType tool) => deformer.Unmask();

			uiComponents.ForEach(ui => ui.Init(Menu));
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
				deformer.Unmask();
		}

		protected abstract SculptState GetState(SculptState prev);
	}
}
