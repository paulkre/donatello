using UnityEngine;

namespace VRSculpting
{
    using SculptMesh;
    using Sculptor;

    public class SculptManager : MonoBehaviour
    {

        [SerializeField]
        public SculptorBehaviour[] sculptors;

        [Header("Mesh")]

        [SerializeField]
        public int subdivisionLevel = 6;

        [SerializeField]
        public float radius = .5f;

        [SerializeField]
        public MeshWrapperBehaviour meshWrapperPrefab;

        private SculptMesh.Modification.SculptMesh sculptMesh;

        public static int FrameCount { get; private set; }

        private void Awake()
        {
            if (meshWrapperPrefab == null) return;

            var meshWrapper = Instantiate(meshWrapperPrefab);
            meshWrapper.Init(subdivisionLevel, radius);
            sculptMesh = meshWrapper.SculptMesh;
        }

        private void Start()
        {
            var menu = new Settings.Menu(Tools.ToolType.Standard);
            if (sculptors != null)
                foreach (var sculptor in sculptors)
                    sculptor.Init(sculptMesh, menu);
        }

        private void Update()
        {
            FrameCount = Time.frameCount;
            sculptMesh.UpdateMeshData();
        }

    }

}
