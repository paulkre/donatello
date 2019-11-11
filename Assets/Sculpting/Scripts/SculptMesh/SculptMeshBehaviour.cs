using UnityEngine;

namespace VRSculpting.SculptMesh {
	
	[RequireComponent(typeof(MeshFilter), typeof(Renderer))]
	public class SculptMeshBehaviour : MonoBehaviour {

		public Material Material { get { return GetComponent<Renderer>().material; } }

		public Mesh Mesh { set { GetComponent<MeshFilter>().sharedMesh = value; } }

	}
}