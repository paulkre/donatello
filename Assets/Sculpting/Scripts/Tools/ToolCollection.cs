using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace VRSculpting.Tools {
	using SculptMesh;
	using Settings;

	public class ToolCollection : ReadOnlyDictionary<ToolType, Tool> {

		public ToolCollection(ISculptMesh sculptMesh, Menu menu) : base(
			new Dictionary<ToolType, Tool> {
				{ ToolType.Standard,  new StandardTool(sculptMesh, menu)},
				{ ToolType.Move,  new MoveTool(sculptMesh, menu)},
				{ ToolType.Smooth,  new SmoothTool(sculptMesh, menu)},
			}
		) { }

	}

}