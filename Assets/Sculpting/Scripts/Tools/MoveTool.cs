using UnityEngine;

namespace VRSculpting.Tools
{
    using SculptMesh.Modification;
    using Sculptor;
    using Settings;

    public class MoveTool : Tool
    {

        private Vector3 prevPosition;

        public MoveTool(SculptMesh mesh) : base(ToolType.Move, mesh)
        {
            prevPosition = Vector3.zero;
        }

        public override void Use(SculptState state, Deformer deformer)
        {
            if (state.drawingDown)
            {
                deformer.UpdateMask(state);
                prevPosition = state.position;
            }

            if (deformer.SelectionCount == 0) return;

            var delta = state.worldToLocal * state.position - state.worldToLocal * prevPosition;

            var deformation = deformer.Deformation;
            for (int i = 0; i < deformer.SelectionCount; ++i)
                deformation[i] = delta;

            prevPosition = state.position;

            deformer.ApplyDeformation();
        }
    }
}
