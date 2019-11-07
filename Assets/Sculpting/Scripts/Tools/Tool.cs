namespace VRSculpting.Tools {
	using SculptMesh;
	using Settings;
	using Sculptor;

	public abstract class Tool {

		public ToolType Type { get; private set; }

		protected ISculptMesh SculptMesh { get; private set; }

		private Menu menu;

		public Tool(ToolType type, ISculptMesh sculptMesh, Menu menu) {
			this.menu = menu;
			Type = type;
			SculptMesh = sculptMesh;
		}

		protected float Size { get { return menu.ToolSize.Value; } }
		protected float Hardness { get { return menu.ToolHardness.Value; } }

		public abstract void Use(SculptState state);
	}
}
