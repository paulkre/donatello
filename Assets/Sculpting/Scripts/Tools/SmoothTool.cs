using System.Collections.Generic;
using UnityEngine;

namespace VRSculpting.Tools
{
    using SculptMesh.Modification;
    using Sculptor;
    using Settings;

    public class SmoothTool : Tool
    {

        private static float strength = .05f;

        public SmoothTool(SculptMesh mesh) : base(ToolType.Smooth, mesh) { }

        public override void Use(SculptState state, Deformer deformer)
        {
            deformer.UpdateMask(state);

            var mask = deformer.Selection;
            var edgeRecorder = new HashSet<int>();

            Vector3[] deformation = deformer.Deformation;
            for (int i = 0; i < deformer.SelectionCount; i++)
            {
                var vert = SculptMesh.Topology.Vertices[mask[i]];
                var force = new Vector3();

                foreach (var edge in vert.Edges)
                {
                    var other = edge.GetOtherVertex(vert);
                    var p0 = SculptMesh.Points[vert.Id];
                    var p1 = SculptMesh.Points[other.Id];
                    var delta = p1 - p0;
                    force += delta;
                }

                deformation[i] = strength * state.strength * force;
            }

            deformer.ApplyDeformation();
        }

    }

}
