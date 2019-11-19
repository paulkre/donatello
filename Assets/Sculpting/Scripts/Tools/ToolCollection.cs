using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace VRSculpting.Tools
{
    using SculptMesh.Modification;
    using Settings;

    public class ToolCollection : ReadOnlyDictionary<ToolType, Tool>
    {

        public ToolCollection(SculptMesh sculptMesh) : base(
            new Dictionary<ToolType, Tool> {
                { ToolType.Standard,  new StandardTool(sculptMesh)},
                { ToolType.Move,  new MoveTool(sculptMesh)},
                { ToolType.Smooth,  new SmoothTool(sculptMesh)},
            }
        )
        { }

    }

}