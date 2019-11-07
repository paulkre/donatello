using System.Collections.Generic;
using UnityEngine;

namespace VRSculpting.Tools {
	using SculptMesh.Modification;
	using Sculptor;
	using Settings;

	public class SmoothTool : Tool {

		private static float strength = .05f;
		
		public SmoothTool(
			SculptMesh mesh,
			Deformer deformer,
			Menu menu
		) : base(ToolType.Smooth, mesh, deformer, menu) { }

		public override void Use(SculptState state) {
			var deformer = Deformer;

			deformer.UpdateMask(state.position, Size / 2, Hardness);

			var mask = deformer.Mask;
			var edgeRecorder = new HashSet<int>();

			Vector3[] deformation = deformer.Deformation;
			for (int i = 0; i < deformer.MaskCount; i++) {
				var vert = SculptMesh.Topology.Vertices[mask[i]];
				var force = new Vector3();

				foreach (var edge in vert.Edges) {
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
