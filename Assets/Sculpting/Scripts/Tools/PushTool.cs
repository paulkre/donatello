using UnityEngine;

namespace VRSculpting.Tools
{
    using SculptMesh.Modification;
    using Sculptor;
    using Settings;

    public class PushTool : Tool
    {

        private static float strength = .001f;

        public PushTool(SculptMesh mesh) : base(ToolType.Push, mesh) { }

        public override void Use(SculptState state, Deformer deformer)
        {
            deformer.UpdateMask(state);

            var mask = deformer.Selection;
            var deformation = deformer.Deformation;

            Vector3 avgNormal = Vector3.zero;
            for (int i = 0; i < deformer.SelectionCount; i++)
                avgNormal += SculptMesh.Normals[mask[i]];
            avgNormal.Normalize();

            float amp = -strength * state.strength;
            float inv = state.drawingInverted ? -1f : 1f;
            for (int i = 0; i < deformer.SelectionCount; ++i)
                deformation[i] = amp * inv * avgNormal;

            deformer.ApplyDeformation();
        }
    }
}
