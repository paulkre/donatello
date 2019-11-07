using UnityEngine;

namespace VRSculpting.SculptMesh {
	using Modification;
	using Modification.Topology;

	public interface ISculptMesh {

		Mesh Mesh { get; }

		Vector3[] Points { get; }
		Vector3[] Normals { get; }
		int[] Ids { get; }

		TopologyManager Topology { get; }

		MeshWrapperBehaviour Wrapper { get; }

		int Select(Vector3 center, float radius, int[] selection);

		void ApplyDeformation(Deformer deformer);

		void HandleDeformation();

		void UpdateMeshData();

	}
}