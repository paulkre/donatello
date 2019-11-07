using UnityEngine;

namespace VRSculpting.SculptMesh.Modification {
	using Topology;

	public class SculptMesh {

		protected Mesh mesh;
		public MeshWrapperBehaviour Wrapper { get; private set; }

		public Mesh Mesh { get { return mesh; } }

		public Vector3[] Points { get; private set; }
		public Vector3[] Normals { get; private set; }
		public int[] Ids { get; private set; }

		public TopologyManager Topology { get; private set; }

		private SpatialContainer spatialContainer;
		
		private Vector3[] deformation;
		private bool[] deformationMask;

		private bool hasDeformation;
		private bool needsUpdate;

		public SculptMesh(MeshWrapperBehaviour wrapper, Mesh mesh) {
			this.mesh = mesh;
			Wrapper = wrapper;

			mesh.name = "Sculpting Mesh";

			Points = mesh.vertices;
			Normals = mesh.normals;
			Ids = mesh.triangles;

			Topology = new TopologyManager(Points, Ids);

			spatialContainer = new SpatialContainer(Points, 3, 16);

			deformation = new Vector3[Points.Length];
			deformationMask = new bool[Points.Length];

			PrintMeshInfo();
		}

		public int Select(Vector3 center, float radius, int[] selection) {
			return spatialContainer.Select(center, radius, selection);
		}
		
		public void HandleDeformation() {
			if (!hasDeformation) return;

			ApplyDeformation();
			UpdateNormals();

			needsUpdate = true;

			ResetDeformation();
		}

		public void ApplyDeformation(Deformer deformer) {
			int[] ids = deformer.Mask;
			float[] weights = deformer.Weights;
			Vector3[] deformation = deformer.Deformation;

			for (int i = 0; i < deformer.MaskCount; ++i) {
				int id = ids[i];
				this.deformation[id] += weights[i] * deformation[i];
				deformationMask[id] = true;
			}

			hasDeformation = true;
		}

		// Synchronizes the half-edge data with the Unity mesh data
		public void UpdateMeshData() {
			if (!needsUpdate) return;

			mesh.vertices = Points;
			mesh.triangles = Ids;
			mesh.normals = Normals;

			needsUpdate = false;
		}

		private void ApplyDeformation() {
			for (int i = 0; i < deformation.Length; ++i) {
				if (!deformationMask[i]) continue;
				Points[i] += deformation[i];
			}
		}

		private void UpdateNormals() {
			for (int i = 0; i < deformation.Length; ++i) {
				if (!deformationMask[i]) continue;
				Normals[i] = Topology.GetNormal(i);
			}
		}

		private void ResetDeformation() {
			for (int i = 0; i < deformation.Length; ++i) {
				if (!deformationMask[i]) continue;
				deformationMask[i] = false;
				deformation[i] = Vector3.zero;
			}

			hasDeformation = false;
		}

		private void PrintMeshInfo() {
			Debug.Log($"Vertex count: {Points.Length}");
			Debug.Log($"Polygon count: {Ids.Length / 3}");
		}

	}
}