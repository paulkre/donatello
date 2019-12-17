using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace VRSculpting.Sculptor
{
    using SculptMesh.Modification;
    using Tools;

    public class ToolCollection : ReadOnlyDictionary<ToolType, Tool>
    {

        public ToolCollection(SculptMesh sculptMesh) : base(
            new Dictionary<ToolType, Tool> {
                { ToolType.Standard,  new StandardTool(sculptMesh)},
                { ToolType.Push,  new PushTool(sculptMesh)},
                { ToolType.Move,  new MoveTool(sculptMesh)},
                { ToolType.Smooth,  new SmoothTool(sculptMesh)},
            }
        )
        { }

    }

}