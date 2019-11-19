namespace VRSculpting.Tools
{
    using SculptMesh.Modification;
    using Settings;
    using Sculptor;

    public abstract class Tool
    {

        public ToolType Type { get; private set; }

        protected SculptMesh SculptMesh { get; private set; }

        public Tool(ToolType type, SculptMesh sculptMesh)
        {
            Type = type;
            SculptMesh = sculptMesh;
        }

        public abstract void Use(SculptState state, Deformer deformer);
    }
}
