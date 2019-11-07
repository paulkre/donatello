using UnityEngine;

namespace VRSculpting.SculptMesh.Modification {
	using Topology;

	public class MultiMesh : ISculptMesh {

		private ISculptMesh sculptMesh;

		public Mesh Mesh { get { return sculptMesh.Mesh; } }

		public MeshWrapperBehaviour Wrapper { get { return sculptMesh.Wrapper; } }

		public Vector3[] Points { get { return sculptMesh.Points; } }
		public Vector3[] Normals { get { return sculptMesh.Normals; } }
		public int[] Ids { get { return sculptMesh.Ids; } }

		public TopologyManager Topology { get { return sculptMesh.Topology; } }

		public Deformer Deformer { get { return sculptMesh.Deformer; } }

		public MultiMesh(MeshWrapperBehaviour wrapper, int subdivisions = 3, float radius = .5f) {
			Mesh mesh = IcoSphereCreator.Create(subdivisions, radius);
			sculptMesh = new StaticMesh(wrapper, mesh);
		}

		public void ApplyDeformation() {
			sculptMesh.ApplyDeformation();
		}

		public void UpdateMeshData() {
			sculptMesh.UpdateMeshData();
		}

	}

}