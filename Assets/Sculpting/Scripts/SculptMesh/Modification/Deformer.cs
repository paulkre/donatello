using UnityEngine;

namespace VRSculpting.SculptMesh.Modification
{
    using Settings;
    using Sculptor;

    public class Deformer
    {

        private SculptMesh mesh;

        public int[] Selection { get; private set; }
        public float[] Weights { get; private set; }
        public Vector3[] Deformation { get; private set; }

        public int SelectionCount { get; private set; }

        public Deformer(SculptMesh mesh)
        {
            this.mesh = mesh;

            int count = mesh.Points.Length;

            Selection = new int[count];
            Weights = new float[count];
            Deformation = new Vector3[count];
            SelectionCount = 0;
        }

        public void UpdateMask(SculptState state)
        {
            Vector3 center = state.worldToLocal.MultiplyPoint(state.position);
            float radius = (state.menuState.toolSize * state.worldToLocal.lossyScale.x) / 2;
            float weightStrength = state.menuState.toolHardness;

            SelectionCount = mesh.Select(center, radius, Selection);

            var points = mesh.Points;
            for (int i = 0; i < SelectionCount; i++)
            {
                var p = points[Selection[i]];
                float t = Mathf.Clamp01(Vector3.Distance(center, p) / radius);
                Weights[i] = 1f - Mathf.Pow(t * t * (3 - 2 * t), weightStrength);
            }
        }

        public void Unmask()
        {
            SelectionCount = 0;
        }

        public void ApplyDeformation()
        {
            mesh.AddDeformation(this);
        }

    }

}
