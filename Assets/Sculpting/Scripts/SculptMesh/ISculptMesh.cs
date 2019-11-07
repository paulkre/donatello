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

		Deformer Deformer { get; }

		void ApplyDeformation();

		void UpdateMeshData();

	}
}