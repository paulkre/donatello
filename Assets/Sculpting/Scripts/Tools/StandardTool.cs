using UnityEngine;

namespace VRSculpting.Tools {
	using SculptMesh;
	using Sculptor;
	using Settings;

	public class StandardTool : Tool {

		private static float strength = .001f;

		public StandardTool(ISculptMesh mesh, Menu menu) : base(ToolType.Standard, mesh, menu) { }

		public override void Use(SculptState state) {
			var deformer = SculptMesh.Deformer;

			deformer.UpdateMask(state.position, Size / 2, Hardness);

			var mask = deformer.Mask;
			var deformation = deformer.Deformation;

			Vector3 avgNormal = Vector3.zero;
			for (int i = 0; i < deformer.MaskCount; i++)
				avgNormal += SculptMesh.Normals[mask[i]];
			avgNormal.Normalize();

			float amp = strength * state.strength;
			float inv = state.drawingInverted ? -1f : 1f;
			for (int i = 0; i < deformer.MaskCount; ++i)
				deformation[i] = amp * inv * avgNormal;

			deformer.ApplyDeformation();
		}
	}
}
