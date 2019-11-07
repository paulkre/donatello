using UnityEngine;

namespace VRSculpting {
	using SculptMesh;
	using Sculptor;

	public class SculptManager : MonoBehaviour {

		[SerializeField]
		private SculptorBehaviour[] sculptors;

		[Header("Mesh")]

		[SerializeField]
		private int subdivisionLevel = 6;

		[SerializeField]
		private float radius = .5f;

		[SerializeField]
		private MeshWrapperBehaviour meshWrapperPrefab;

		private ISculptMesh sculptMesh;

		private void Awake() {
			if (meshWrapperPrefab == null) return;

			var meshWrapper = Instantiate(meshWrapperPrefab);
			meshWrapper.Init(subdivisionLevel, radius);
			sculptMesh = meshWrapper.SculptMesh;

			if (sculptors != null)
				foreach (var sculptor in sculptors) sculptor.Init(sculptMesh);
		}

		private void Update() {
			if (sculptors == null) return;

			foreach (var s in sculptors)
				s.Sculpt();

			sculptMesh.ApplyDeformation();
			sculptMesh.UpdateMeshData();
		}
	}
}
