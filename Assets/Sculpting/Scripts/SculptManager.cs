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

		private SculptMesh.Modification.SculptMesh sculptMesh;

		public static int FrameCount { get; private set; }

		private void Awake() {
			if (meshWrapperPrefab == null) return;

			var meshWrapper = Instantiate(meshWrapperPrefab);
			meshWrapper.Init(subdivisionLevel, radius);
			sculptMesh = meshWrapper.SculptMesh;
		}

		private void Start() {
			if (sculptors != null)
				foreach (var sculptor in sculptors)
					sculptor.Init(sculptMesh);
		}

		private void Update() {
			FrameCount = Time.frameCount;
			sculptMesh.UpdateMeshData();
		}

	}

}
