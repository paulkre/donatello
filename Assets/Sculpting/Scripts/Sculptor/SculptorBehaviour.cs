using System.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace VRSculpting.Sculptor {
	using Tools;
	using SculptMesh;
	using SculptMesh.Modification;
	using Settings;

	public abstract class SculptorBehaviour : MonoBehaviour {

		[SerializeField]
		public List<UI.UI> uiComponents;

		protected Menu Menu { get; private set; }

		private ToolCollection mainColl;

		private SculptState currentState;
		private Stack<SculptState> stateStack;

		protected MeshWrapperBehaviour MeshWrapper { get; private set; }

		private Deformer deformer;

		private bool running;

		public virtual void Init(SculptMesh sculptMesh) {
			MeshWrapper = sculptMesh.Wrapper;

			Menu = new Menu(ToolType.Standard);

			deformer = new Deformer(sculptMesh);

			mainColl = new ToolCollection(sculptMesh, deformer);

			uiComponents.ForEach(ui => ui.Init(Menu));

			stateStack = new Stack<SculptState>();

			running = true;
			new Thread(
				new ThreadStart(SculptLoop)
			).Start();
		}

		private void Update() {
			currentState = GetState(currentState);
			currentState.worldToLocal = MeshWrapper.MeshTransform.worldToLocalMatrix;
			currentState.menuState = Menu.GetState();

			var mat = MeshWrapper.Material;
			mat.SetVector($"_BrushPos", currentState.position);
			mat.SetFloat($"_BrushRadius", Menu.ToolSize.Value / 2);
			mat.SetFloat($"_BrushHardness", Menu.ToolHardness.Value);

			stateStack.Push(currentState);
		}

		private void SculptLoop() {
			while (running)
				while (stateStack.Count > 0)
					Sculpt(stateStack.Pop());
		}

		private void Sculpt(SculptState state) {
			if (state.drawing)
				mainColl[state.menuState.tool].Use(state);
			else if (state.drawingUp)
				deformer.Unmask();

			MeshWrapper.SculptMesh.ApplyDeformation();
		}
		
		protected abstract SculptState GetState(SculptState prev);
	}
}
