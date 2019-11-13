namespace VRSculpting.Tools
{
    using SculptMesh.Modification;
    using Settings;
    using Sculptor;

    public abstract class Tool
    {

        public ToolType Type { get; private set; }

        protected SculptMesh SculptMesh { get; private set; }

        protected Deformer Deformer { get; private set; }

        public Tool(ToolType type, SculptMesh sculptMesh, Deformer deformer)
        {
            Type = type;
            SculptMesh = sculptMesh;
            Deformer = deformer;
        }

        public abstract void Use(SculptState state);
    }
}
