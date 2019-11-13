using System.Collections.Generic;
using UnityEngine;

namespace VRSculpting.SculptMesh.Modification.Topology
{

    public class TopologyManager
    {

        public Vertex[] Vertices { get; private set; }

        public Edge[] Edges { get; private set; }

        public NormalCalculator NormalCalculator { get; private set; }

        public TopologyManager(Vector3[] points, int[] ids)
        {
            Vertices = new Vertex[points.Length];

            var edges = new List<Edge>();
            var edgeRecorder = new HashSet<long>();

            for (int i = 0; i < points.Length; i++)
                Vertices[i] = new Vertex(i);

            for (int i = 0; i < ids.Length; i += 3)
            {
                var faceId = i / 3;

                for (int j = 0; j < 3; j++)
                {
                    var v0 = Vertices[ids[i + j]];
                    var v1 = Vertices[ids[i + (j + 1) % 3]];

                    var hash = Edge.GetHash(v0, v1);

                    if (!edgeRecorder.Contains(hash))
                    {
                        edges.Add(new Edge(v0, v1, edges.Count));
                        edgeRecorder.Add(hash);
                    }

                    v0.Faces.AddLast(faceId);
                }
            }

            NormalCalculator = new NormalCalculator(Vertices, points, ids);

            Edges = edges.ToArray();
        }

    }

}