using UnityEngine;

namespace VRSculpting.SculptMesh.Modification {

	public class Deformer {

		private SculptMesh mesh;

		public int[] Mask { get; private set; }
		public float[] Weights { get; private set; }
		public Vector3[] Deformation { get; private set; }

		public int MaskCount { get; private set; }

		public Deformer(SculptMesh mesh) {
			this.mesh = mesh;
			Mask = new int[mesh.Points.Length];
			Weights = new float[mesh.Points.Length];
			Deformation = new Vector3[mesh.Points.Length];
			MaskCount = 0;
		}

		public void UpdateMask(Vector3 center, float radius, float weightStrength = 2f) {
			center = mesh.WorldToLocal(center);
			radius = mesh.WorldToLocal(radius);

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
	}

}
