using UnityEngine;

namespace VRSculpting.SculptMesh {
	
	public class MeshWrapperBehaviour : MonoBehaviour {

		private bool initialized = false;

		[SerializeField]
		public SculptMeshBehaviour sculptMeshReference;

		public Modification.SculptMesh SculptMesh { get; private set; }

		public Material Material { get { return sculptMeshReference.Material; } }

		public Transform MeshTransform { get { return sculptMeshReference.transform; } }

		public void Init(int subdivisionLevel = 6, float radius = .5f) {
			if (sculptMeshReference == null || initialized) return;

			var mesh = IcoSphereCreator.Create(subdivisionLevel, radius);

			SculptMesh = new Modification.SculptMesh(this, mesh);
			sculptMeshReference.Mesh = SculptMesh.Mesh;

			initialized = true;
		}

		public void ApplyRotation() {
			var tmp = MeshTransform.rotation;
			transform.rotation = Quaternion.Euler(Vector3.zero);
			sculptMeshReference.transform.rotation = tmp;
		}

	}
}