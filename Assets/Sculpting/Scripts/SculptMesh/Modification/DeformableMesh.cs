using System.Threading;
using UnityEngine;

namespace VRSculpting.SculptMesh.Modification {
	using Topology;

	public class DeformableMesh {

		public Vector3[] Points { get; private set; }
		public int[] Ids { get; private set; }
		public Vector3[] Normals { get; private set; }

		public TopologyManager Topology { get; private set; }
		private SpatialContainer spatialContainer;

		private Vector3[] deformation;
		private bool[] deformationMask;

		private NormalCalculator normalCalculator;
		private bool[] normalsToUpdateMask;

		private bool hasDeformation;

		public bool IsAddingDeformation { get; private set; }

		private bool running;

		protected bool PointsNeedUpdate { get; set; }
		protected bool NormalsNeedUpdate { get; set; }

		public DeformableMesh(Vector3[] points, int[] ids, Vector3[] normals) {
			Points = points;
			Ids = ids;
			Normals = normals;

			Topology = new TopologyManager(points, ids);
			spatialContainer = new SpatialContainer(points, 3, 16);

			normalCalculator = new NormalCalculator(Topology.Vertices, points, ids);

			deformation = new Vector3[points.Length];
			deformationMask = new bool[points.Length];
			normalsToUpdateMask = new bool[points.Length];

			running = true;
			new Thread(
				new ThreadStart(UpdateNormalsLoop)
			).Start();
		}

		private void UpdateNormalsLoop() {
			while (running) {
				var didUpdate = normalCalculator.UpdateNormals(
					Normals,
					normalsToUpdateMask
				);
				if (didUpdate) NormalsNeedUpdate = true;
			}
		}

		public int Select(Vector3 center, float radius, int[] selection) {
			return spatialContainer.Select(center, radius, selection);
		}

		public void AddDeformation(Deformer deformer) {
			IsAddingDeformation = true;

			int[] ids = deformer.Mask;
			float[] weights = deformer.Weights;
			Vector3[] deformation = deformer.Deformation;

			for (int i = 0; i < deformer.MaskCount; ++i) {
				int id = ids[i];
				this.deformation[id] += weights[i] * deformation[i];
				deformationMask[id] = true;
			}

			hasDeformation = true;

			IsAddingDeformation = false;
		}

		public void ApplyDeformation() {
			if (!hasDeformation) return;

			UpdatePoints();
			MarkNormalsToUpdate();
			spatialContainer.UpdatePoints(deformationMask);
			ResetDeformation();
		}

		private void MarkNormalsToUpdate() {
			for (int i = 0; i < deformationMask.Length; i++) {
				if (deformationMask[i])
					normalsToUpdateMask[i] = true;
			}
		}

		private void UpdatePoints() {
			for (int i = 0; i < deformation.Length; ++i) {
				if (deformationMask[i])
					Points[i] += deformation[i];
			}

			PointsNeedUpdate = true;
		}

		private void ResetDeformation() {
			for (int i = 0; i < deformation.Length; ++i) {
				if (!deformationMask[i]) continue;
				deformationMask[i] = false;
				deformation[i] = Vector3.zero;
			}

			hasDeformation = false;
		}

	}

}