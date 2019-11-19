using UnityEngine;

namespace VRSculpting
{
    using Helpers;
    using SculptMesh;
    using Sculptor;

    public class SculptManager : MonoBehaviour
    {
        
        public SculptorBehaviour[] sculptors;

        public MeshWrapperBehaviour meshWrapper;

        public static int FrameCount { get; private set; }

        private void Start()
        {
            if (meshWrapper == null) return;

            var menu = new Settings.Menu(Tools.ToolType.Standard);
            if (sculptors != null)
                foreach (var sculptor in sculptors)
                    sculptor.Init(meshWrapper.SculptMesh, menu);

            menu.ExportAction.OnDone += () =>
            {
                ObjExporter.Export(
                    meshWrapper.SculptMesh.Mesh,
                    meshWrapper.MeshTransform
                );
            };
        }

        private void Update()
        {
            FrameCount = Time.frameCount;
            meshWrapper.SculptMesh.UpdateMeshData();
        }

    }

}
