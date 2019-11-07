using System.Collections.Generic;
using UnityEngine;

namespace VRSculpting.SculptMesh.Modification {
	using Topology;

	public class SculptMesh : ISculptMesh {

		protected Mesh mesh;
		public MeshWrapperBehaviour Wrapper { get; private set; }

		public Mesh Mesh { get { return mesh; } }

		public Vector3[] Points { get; private set; }
		public Vector3[] Normals { get; private set; }
		public int[] Ids { get; private set; }

		public TopologyManager Topology { get; private set; }

		private SpatialContainer spatialContainer;

		public Deformer Deformer { get; private set; }

		public SculptMesh(MeshWrapperBehaviour wrapper, Mesh mesh) {
			this.mesh = mesh;
			Wrapper = wrapper;

			mesh.name = "Sculpting Mesh";

			Points = mesh.vertices;
			Normals = mesh.normals;
			Ids = mesh.triangles;

			Topology = new TopologyManager(Points, Ids);

			spatialContainer = new SpatialContainer(Points, 3, 16);

			Deformer = new Deformer(this);

			PrintMeshInfo();
		}

		public Vector3 WorldToLocal(Vector3 point) {
			return Wrapper.MeshTransform.InverseTransformPoint(point);
		}

		public float WorldToLocal(float scalar) {
			return scalar / Wrapper.transform.localScale.x;
		}

		public int Select(Vector3 center, float radius, int[] selection) {
			return spatialContainer.Select(center, radius, selection);
		}

		// Applies all saved deformation fields to the mesh.
		public void ApplyDeformation() {
			int[] mask = Deformer.Mask;
			float[] weights = Deformer.Weights;
			Vector3[] deformation = Deformer.Deformation;

			for (int i = 0; i < Deformer.MaskCount; ++i)
				Points[mask[i]] += weights[i] * deformation[i];

			spatialContainer.UpdatePoints(mask, Deformer.MaskCount);
			UpdateNormals(mask, Deformer.MaskCount);
		}

		// Synchronizes the half-edge data with the Unity mesh data
		public void UpdateMeshData() {
			mesh.vertices = Points;
			mesh.triangles = Ids;
			mesh.normals = Normals;
		}

		private void UpdateNormals(int[] mask, int length) {
			var verts = Topology.Vertices;
			for (int i = 0; i < length; i++) {
				int id = mask[i];
				Normals[id] = Topology.GetNormal(id);
			}
		}

		private void PrintMeshInfo() {
			Debug.Log($"Vertex count: {Points.Length}");
			Debug.Log($"Polygon count: {Ids.Length / 3}");
		}

	}
}