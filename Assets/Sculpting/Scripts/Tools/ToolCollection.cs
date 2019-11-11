using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace VRSculpting.Tools {
	using SculptMesh.Modification;
	using Settings;

	public class ToolCollection : ReadOnlyDictionary<ToolType, Tool> {

		public ToolCollection(SculptMesh sculptMesh, Deformer deformer) : base(
			new Dictionary<ToolType, Tool> {
				{ ToolType.Standard,  new StandardTool(sculptMesh, deformer)},
				{ ToolType.Move,  new MoveTool(sculptMesh, deformer)},
				{ ToolType.Smooth,  new SmoothTool(sculptMesh, deformer)},
			}
		) { }

	}

}