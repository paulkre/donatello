using System.Collections.Generic;

namespace VRSculpting.SculptMesh.Modification.Topology {

	public class Vertex {

		public int Id { get; private set; }

		public LinkedList<Edge> Edges { get; private set; }

		public LinkedList<int> Faces { get; private set; }

		public Vertex(int id) {
			Id = id;
			Edges = new LinkedList<Edge>();
			Faces = new LinkedList<int>();
		}

	}

}