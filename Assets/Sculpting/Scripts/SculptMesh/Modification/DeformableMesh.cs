using System;
using System.Threading;
using UnityEngine;

namespace VRSculpting.SculptMesh.Modification {
	using Topology;

	public class DeformableMesh {

		public Vector3[] Points { get; private set; }
		public int[] Ids { get; private set; }
		public Vector3[] Normals { get; private set; }

		public TopologyManager Topology { get; private set; }
		private NormalCalculator normalCalculator;
		private SpatialContainer spatialContainer;

		private Vector3[] deformation;
		private bool[] deformationMask;

		private bool hasDeformation;
		protected bool needsUpdate;

		private bool deformationThreadIsActive;

		private bool isAddingDeformation;
		private bool isApplyingDeformation;
		private bool isReadingDeformation;

		private Vector3[] deformationCopy;
		private bool[] deformationMaskCopy;

		public DeformableMesh(Vector3[] points, int[] ids, Vector3[] normals) {
			Points = points;
			Ids = ids;
			Normals = normals;

			Topology = new TopologyManager(points, ids);
			normalCalculator = new NormalCalculator(points, ids);
			spatialContainer = new SpatialContainer(points, 3, 16);

			deformation = new Vector3[points.Length];
			deformationMask = new bool[points.Length];

			deformationCopy = new Vector3[points.Length];
			deformationMaskCopy = new bool[points.Length];

			deformationThreadIsActive = true;

			new Thread(
				new ThreadStart(RunDeformationHandling)
			).Start();
		}

		public int Select(Vector3 center, float radius, int[] selection) {
			while (isApplyingDeformation) { }

			return spatialContainer.Select(center, radius, selection);
		}

		public void ApplyDeformation(Deformer deformer) {
			while (isReadingDeformation) { }

			isAddingDeformation = true;

			int[] ids = deformer.Mask;
			float[] weights = deformer.Weights;
			Vector3[] deformation = deformer.Deformation;

			for (int i = 0; i < deformer.MaskCount; ++i) {
				int id = ids[i];
				this.deformation[id] += weights[i] * deformation[i];
				deformationMask[id] = true;
			}

			hasDeformation = true;

			isAddingDeformation = false;
		}

		private void RunDeformationHandling() {
			while (deformationThreadIsActive)
				HandleDeformation();
		}

		private void HandleDeformation() {
			if (!hasDeformation) return;

			while (isAddingDeformation) { }

			isApplyingDeformation = true;

			isReadingDeformation = true;
			CopyDeformation();
			ResetDeformation();
			isReadingDeformation = false;

			ApplyDeformation();
			spatialContainer.UpdatePoints(deformationMaskCopy);

			isApplyingDeformation = false;

			UpdateNormals();

			needsUpdate = true;
		}

		private void ApplyDeformation() {
			for (int i = 0; i < deformation.Length; ++i) {
				if (!deformationMaskCopy[i]) continue;
				Points[i] += deformationCopy[i];
			}
		}

		private void UpdateNormals() {
			normalCalculator.executionCount++;
			var verts = Topology.Vertices;
			for (int i = 0; i < deformationCopy.Length; ++i) {
				if (!deformationMaskCopy[i]) continue;
				Normals[i] = normalCalculator.GetNormal(verts[i]);
			}
		}

		private void CopyDeformation() {
			Array.Copy(deformation, deformationCopy, deformation.Length);
			Array.Copy(deformationMask, deformationMaskCopy, deformation.Length);
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