using UnityEngine;

namespace VRSculpting.SculptMesh.Modification {
	using Settings;
	using Sculptor;

	public class Deformer {

		private SculptMesh mesh;
		private Menu menu;

		public int[] Mask { get; private set; }
		public float[] Weights { get; private set; }
		public Vector3[] Deformation { get; private set; }

		public int MaskCount { get; private set; }

		public Deformer(SculptMesh mesh, Menu menu) {
			this.mesh = mesh;
			this.menu = menu;

			int count = mesh.Points.Length;

			Mask = new int[count];
			Weights = new float[count];
			Deformation = new Vector3[count];
			MaskCount = 0;
		}

		public void UpdateMask(SculptState state) {
			Vector3 center = state.worldToLocal.MultiplyPoint(state.position);
			float radius = (menu.ToolSize.Value * state.worldToLocal.lossyScale.x) / 2;
			float weightStrength = menu.ToolHardness.Value;
			
			MaskCount = mesh.Select(center, radius, Mask);

			var points = mesh.Points;
			for (int i = 0; i < MaskCount; i++) {
				var p = points[Mask[i]];
				float t = Mathf.Clamp01(Vector3.Distance(center, p) / radius);
				Weights[i] = 1f - Mathf.Pow(t * t * (3 - 2 * t), weightStrength);
			}
		}

		public void Unmask() {
			MaskCount = 0;
		}

		public void ApplyDeformation() {
			mesh.AddDeformation(this);
		}

	}

}
