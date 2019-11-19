using UnityEngine;

namespace VRSculpting.SculptMesh
{

    public class MeshWrapperBehaviour : MonoBehaviour
    {
        public SculptMeshBehaviour sculptMeshReference;

        public int subdivisionLevel = 6;

        public float radius = .5f;

        private bool initialized = false;

        public Modification.SculptMesh SculptMesh { get; private set; }

        public Material Material { get { return sculptMeshReference.Material; } }

        public Transform MeshTransform { get { return sculptMeshReference.transform; } }

        public void Awake()
        {
            if (sculptMeshReference == null || initialized) return;

            var mesh = IcoSphereCreator.Create(subdivisionLevel, radius);

            SculptMesh = new Modification.SculptMesh(this, mesh);
            sculptMeshReference.Mesh = SculptMesh.Mesh;

            initialized = true;
        }

        public void ApplyRotation()
        {
            var tmp = MeshTransform.rotation;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            sculptMeshReference.transform.rotation = tmp;
        }

    }
}