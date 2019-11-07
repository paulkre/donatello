using System.Collections.Generic;
using UnityEngine;

namespace VRSculpting.SculptMesh {
	public static class IcoSphereCreator {

		public static Mesh Create(int subdivisions = 3, float radius = .5f) {
			var points = new List<Vector3>();
			var ids = new List<int>();
			var midPointCache = new Dictionary<long, int>();

			CreateIcosahedron(ref points, ref ids, radius);

			// Refine triangles.
			for (int i = 0; i < subdivisions; ++i) {
				var sids = new List<int>();

				for (int j = 0; j < ids.Count; j += 3) {
					int[] mids = new int[3];

					for (int k = 0; k < 3; ++k)
						mids[k] = GetMidPoint(ids[j + k], ids[j + (k + 1) % 3], ref points, ref midPointCache, radius);

					for (int k = 0; k < 3; ++k)
						CreateFace(ids[j + k], mids[k], mids[(k + 2) % 3], ref sids);

					CreateFace(mids[0], mids[1], mids[2], ref sids);
				}

				ids = sids;
			}

			var normals = points.ToArray();
			for (int i = 0; i < normals.Length; ++i)
				normals[i].Normalize();

			return new Mesh {
				indexFormat = UnityEngine.Rendering.IndexFormat.UInt32,
				vertices = points.ToArray(),
				triangles = ids.ToArray(),
				normals = normals
			};
		}

		private static void CreateIcosahedron(ref List<Vector3> points, ref List<int> ids, float rad) {
			float t = (1f + Mathf.Sqrt(5f)) / 2f;

			// create 12 vertices of a icosahedron
			points.Add(new Vector3(-1f, t, 0f).normalized * rad);
			points.Add(new Vector3(1f, t, 0f).normalized * rad);
			points.Add(new Vector3(-1f, -t, 0f).normalized * rad);
			points.Add(new Vector3(1f, -t, 0f).normalized * rad);

			points.Add(new Vector3(0f, -1f, t).normalized * rad);
			points.Add(new Vector3(0f, 1f, t).normalized * rad);
			points.Add(new Vector3(0f, -1f, -t).normalized * rad);
			points.Add(new Vector3(0f, 1f, -t).normalized * rad);

			points.Add(new Vector3(t, 0f, -1f).normalized * rad);
			points.Add(new Vector3(t, 0f, 1f).normalized * rad);
			points.Add(new Vector3(-t, 0f, -1f).normalized * rad);
			points.Add(new Vector3(-t, 0f, 1f).normalized * rad);

			// 5 faces around point 0
			CreateFace(0, 11, 5, ref ids);
			CreateFace(0, 5, 1, ref ids);
			CreateFace(0, 1, 7, ref ids);
			CreateFace(0, 7, 10, ref ids);
			CreateFace(0, 10, 11, ref ids);

			// 5 adjacent faces
			CreateFace(1, 5, 9, ref ids);
			CreateFace(5, 11, 4, ref ids);
			CreateFace(11, 10, 2, ref ids);
			CreateFace(10, 7, 6, ref ids);
			CreateFace(7, 1, 8, ref ids);

			// 5 faces around point 3
			CreateFace(3, 9, 4, ref ids);
			CreateFace(3, 4, 2, ref ids);
			CreateFace(3, 2, 6, ref ids);
			CreateFace(3, 6, 8, ref ids);
			CreateFace(3, 8, 9, ref ids);

			// 5 adjacent faces
			CreateFace(4, 9, 5, ref ids);
			CreateFace(2, 4, 11, ref ids);
			CreateFace(6, 2, 10, ref ids);
			CreateFace(8, 6, 7, ref ids);
			CreateFace(9, 8, 1, ref ids);
		}

		private static void CreateFace(int v0, int v1, int v2, ref List<int> ids) {
			ids.Add(v0);
			ids.Add(v1);
			ids.Add(v2);
		}

		// Return index of point in the middle of i0 and i1.
		private static int GetMidPoint(int v0, int v1, ref List<Vector3> points, ref Dictionary<long, int> cache, float rad) {
			// First check if we have it already.
			bool firstIsSmaller = v0 < v1;
			long sm = firstIsSmaller ? v0 : v1;
			long lg = firstIsSmaller ? v1 : v0;
			long key = (sm << 32) + lg;

			if (cache.TryGetValue(key, out int ret))
				return ret;

			// If not in cache, create the point.
			var p0 = points[v0];
			var p1 = points[v1];

			var mid = new Vector3(
				(p0.x + p1.x) / 2f,
				(p0.y + p1.y) / 2f,
				(p0.z + p1.z) / 2f
			);

			int i = points.Count;
			points.Add(mid.normalized * rad);

			cache.Add(key, i);

			return i;
		}
		
	}
}