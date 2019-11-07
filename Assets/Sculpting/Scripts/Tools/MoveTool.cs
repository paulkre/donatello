﻿using UnityEngine;

namespace VRSculpting.Tools {
	using SculptMesh;
	using SculptMesh.Modification;
	using Sculptor;
	using Settings;

	public class MoveTool : Tool {

		private Vector3 prevPosition;

		public MoveTool(
			ISculptMesh mesh,
			Deformer deformer,
			Menu menu
		) : base(ToolType.Move, mesh, deformer, menu) {
			prevPosition = Vector3.zero;
		}

		public override void Use(SculptState state) {
			var deformer = Deformer;

			if (state.drawingDown) {
				deformer.UpdateMask(
					state.position,
					Size / 2,
					Hardness
				);
				prevPosition = state.position;
			}

			if (deformer.MaskCount == 0) return;

			var trm = SculptMesh.Wrapper.MeshTransform;
			var delta = trm.InverseTransformPoint(state.position) - trm.InverseTransformPoint(prevPosition);

			var deformation = deformer.Deformation;
			for (int i = 0; i < deformer.MaskCount; ++i)
				deformation[i] = delta;

			prevPosition = state.position;

			deformer.ApplyDeformation();
		}
	}
}
