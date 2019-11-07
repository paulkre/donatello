namespace VRSculpting.Tools {
	using SculptMesh;
	using SculptMesh.Modification;
	using Settings;
	using Sculptor;

	public abstract class Tool {

		public ToolType Type { get; private set; }

		protected ISculptMesh SculptMesh { get; private set; }

		protected Deformer Deformer { get; private set; }

		private Menu menu;

		public Tool(ToolType type, ISculptMesh sculptMesh, Deformer deformer,  Menu menu) {
			this.menu = menu;
			Type = type;
			SculptMesh = sculptMesh;
			Deformer = deformer;
		}

		protected float Size { get { return menu.ToolSize.Value; } }
		protected float Hardness { get { return menu.ToolHardness.Value; } }

		public abstract void Use(SculptState state);
	}
}
