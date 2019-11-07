using UnityEngine;

using IndexCollection = System.Collections.Generic.LinkedList<int>;
using IndexNode = System.Collections.Generic.LinkedListNode<int>;

namespace VRSculpting.SculptMesh.Modification {

	class SpatialContainer {

		private Vector3[] points;

		private float min, max;

		private float size, halfSize, cellSize;

		private int subdivisions;

		IndexCollection[][][] collections;
		IndexNode[] indexNodes;

		public SpatialContainer(Vector3[] points, float size, int subdivisions) {
			this.points = points;
			this.size = size;
			this.subdivisions = subdivisions;
			halfSize = size / 2;
			cellSize = size / subdivisions;

			collections = CreateCollections(subdivisions);
			indexNodes = new IndexNode[points.Length];

			FillCollections(points);
		}

		public int Select(Vector3 center, float radius, int[] selection) {
			var start = PointToIndex(
				center.x - radius,
				center.y - radius,
				center.z - radius
			);

			var end = PointToIndex(
				center.x + radius,
				center.y + radius,
				center.z + radius
			);

			int count = 0;

			for (int x = start.x; x <= end.x; x++)
				for (int y = start.y; y <= end.y; y++)
					for (int z = start.z; z <= end.z; z++) {
						foreach (var i in collections[x][y][z]) {
							var p = points[i];
							float dx = p.x - center.x;
							float dy = p.y - center.y;
							float dz = p.z - center.z;
							float d = dx * dx + dy * dy + dz * dz;
							if (d < radius * radius) {
								selection[count] = i;
								count++;
							}
						}
					}

			return count;
		}

		public void UpdatePoints(int[] indices, int length) {
			for (int i = 0; i < length; i++) {
				int id = indices[i];
				var node = indexNodes[id];
				Vector3Int p = PointToIndex(points[id]);
				var coll = collections[p.x][p.y][p.z];
				if (node.List != coll) {
					node.List.Remove(node);
					coll.AddLast(node);
				}
			}
		}

		private void FillCollections(Vector3[] points) {
			for (int i = 0; i < points.Length; i++) {
				Vector3Int p = PointToIndex(points[i]);
				indexNodes[i] = collections[p.x][p.y][p.z].AddLast(i);
			}
		}

		private Vector3Int PointToIndex(Vector3 point) {
			return PointToIndex(point.x, point.y, point.z);
		}

		private Vector3Int PointToIndex(float x, float y, float z) {
			return new Vector3Int(
				DimensionToIndex(x),
				DimensionToIndex(y),
				DimensionToIndex(z)
			);
		}

		private int DimensionToIndex(float dim) {
			return Mathf.Clamp(
				Mathf.FloorToInt(subdivisions * ((dim + halfSize) / size)),
				0,
				subdivisions - 1
			);
		}

		private static IndexCollection[][][] CreateCollections(int count) {
			var collections = new IndexCollection[count][][];

			for (int x = 0; x < count; x++) {
				collections[x] = new IndexCollection[count][];
				for (int y = 0; y < count; y++) {
					collections[x][y] = new IndexCollection[count];
					for (int z = 0; z < count; z++)
						collections[x][y][z] = new IndexCollection();
				}
			}

			return collections;
		}

	}

}
