using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace VRSculpting.Tools {
	using SculptMesh.Modification;
	using Settings;

	public class ToolCollection : ReadOnlyDictionary<ToolType, Tool> {

		public ToolCollection(SculptMesh sculptMesh, Deformer deformer, Menu menu) : base(
			new Dictionary<ToolType, Tool> {
				{ ToolType.Standard,  new StandardTool(sculptMesh, deformer, menu)},
				{ ToolType.Move,  new MoveTool(sculptMesh, deformer, menu)},
				{ ToolType.Smooth,  new SmoothTool(sculptMesh, deformer, menu)},
			}
		) { }

	}

}