using UnityEngine;

namespace VRSculpting.SculptMesh.Modification {

	public class SculptMesh : DeformableMesh {

		protected Mesh mesh;
		public MeshWrapperBehaviour Wrapper { get; private set; }

		public Mesh Mesh { get { return mesh; } }

		public SculptMesh(
			MeshWrapperBehaviour wrapper,
			Mesh mesh
		) : base(
			mesh.vertices,
			mesh.triangles,
			mesh.normals
		) {
			this.mesh = mesh;
			Wrapper = wrapper;

			mesh.name = "Sculpting Mesh";

			PrintMeshInfo();
		}

		// Synchronizes the half-edge data with the Unity mesh data
		public void UpdateMeshData() {
			if (PointsNeedUpdate) {
				PointsNeedUpdate = false;
				mesh.vertices = Points;
			}

			if (NormalsNeedUpdate) {
				NormalsNeedUpdate = false;
				mesh.normals = Normals;
			}
		}

		private void PrintMeshInfo() {
			Debug.Log($"Vertex count: {Points.Length}");
			Debug.Log($"Polygon count: {Ids.Length / 3}");
		}

	}
}