namespace VRSculpting.SculptMesh.Modification.Topology
{

    public class Edge
    {

        public Vertex From { get; private set; }
        public Vertex To { get; private set; }

        public int Id { get; private set; }

        public Edge(Vertex from, Vertex to, int id)
        {
            From = from;
            To = to;

            Id = id;

            From.Edges.AddLast(this);
            To.Edges.AddLast(this);
        }

        public Vertex GetOtherVertex(Vertex vert)
        {
            return vert == From ? To : From;
        }

        public static long GetHash(Vertex v0, Vertex v1)
        {
            int i0 = v0.Id;
            int i1 = v1.Id;

            bool firstIsSmaller = i0 < i1;
            long sm = firstIsSmaller ? i0 : i1;
            long lg = firstIsSmaller ? i1 : i0;
            return (sm << 32) + lg;
        }

    }

}