using System.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace VRSculpting.Sculptor
{
    using Tools;
    using SculptMesh;
    using SculptMesh.Modification;
    using Settings;

    public abstract class SculptorBehaviour : MonoBehaviour
    {
        public List<UI.UI> uiComponents;

        private ToolCollection tools;
        private ToolCollection mirrorTools;

        private SculptState currentState;
        private Stack<SculptState> stateStack;

        protected Menu Menu { get; private set; }
        protected MeshWrapperBehaviour MeshWrapper { get; private set; }

        private Deformer deformer;
        private Deformer mirrorDeformer;

        private bool running;

        public virtual void Init(SculptMesh sculptMesh, Menu menu)
        {
            MeshWrapper = sculptMesh.Wrapper;

            Menu = menu;

            deformer = new Deformer(sculptMesh);
            mirrorDeformer = new Deformer(sculptMesh);

            tools = new ToolCollection(sculptMesh);
            mirrorTools = new ToolCollection(sculptMesh);

            uiComponents.ForEach(ui => ui.Init(Menu));

            stateStack = new Stack<SculptState>();

            running = true;
            new Thread(
                new ThreadStart(SculptLoop)
            ).Start();
        }

        private void Update()
        {
            currentState = GetState(currentState);
            currentState.worldToLocal = MeshWrapper.MeshTransform.worldToLocalMatrix;
            currentState.menuState = Menu.GetState();

            var mat = MeshWrapper.Material;
            mat.SetVector($"_BrushPos", currentState.position);
            mat.SetFloat($"_BrushRadius", Menu.ToolSize.Value / 2);
            mat.SetFloat($"_BrushHardness", Menu.ToolHardness.Value);
            mat.SetFloat($"_MenuEnabled", Menu.AppMenuEnabled.Value ? 1 : 0);

            mat.SetFloat($"_SymmetryEnabled", Menu.SymmetryEnabled.Value ? 1 : 0);
            mat.SetVector($"_BrushPosMirrored", GetMirroredPosition(currentState));

            stateStack.Push(currentState);
        }

        private void SculptLoop()
        {
            while (running)
                while (stateStack.Count > 0)
                    Sculpt(stateStack.Pop());
        }

        private void Sculpt(SculptState state)
        {
            if (state.menuState.appMenuEnabled) return;

            if (state.drawing)
            {
                tools[state.menuState.tool].Use(state, deformer);
                if (state.menuState.symmetryEnabled)
                {
                    state.position = GetMirroredPosition(state);
                    mirrorTools[state.menuState.tool].Use(state, mirrorDeformer);
                }
            }
            else if (state.drawingUp)
            {
                deformer.Unmask();
                mirrorDeformer.Unmask();
            }

            MeshWrapper.SculptMesh.ApplyDeformation();
        }

        private static Vector3 GetMirroredPosition(SculptState state)
        {
            var p = state.position;
            p = state.worldToLocal.MultiplyPoint(p);
            p.x *= -1;
            return state.worldToLocal.inverse.MultiplyPoint(p);
        }

        protected abstract SculptState GetState(SculptState prev);
    }
}
