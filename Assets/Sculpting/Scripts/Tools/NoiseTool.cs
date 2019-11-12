using UnityEngine;

namespace VRSculpting.Tools {
	using SculptMesh.Modification;
	using Sculptor;
	using Helpers;

	public class NoiseTool : Tool {

		private static float strength = .001f;
		private static float noiseScale = 15f;

		private Vector3 prevPosition;

		public NoiseTool(SculptMesh mesh, Deformer deformer) : base(ToolType.Noise, mesh, deformer) { }

		public override void Use(SculptState state) {
			var deformer = Deformer;

			deformer.UpdateMask(state);

			var mask = deformer.Mask;
			var deformation = deformer.Deformation;

			Vector3 avgNormal = Vector3.zero;
			for (int i = 0; i < deformer.MaskCount; i++)
				avgNormal += SculptMesh.Normals[mask[i]];
			avgNormal.Normalize();

			float amp = strength * state.strength;
			float inv = state.drawingInverted ? -1f : 1f;
			for (int i = 0; i < deformer.MaskCount; ++i) {
				float noise = Perlin.Noise(noiseScale * SculptMesh.Points[mask[i]]);
				deformation[i] = noise * amp * inv * avgNormal;
			}

			deformer.ApplyDeformation();
		}
	}
}
