using UnityEngine;

namespace VRSculpting.SculptMesh.Modification.Topology {

	public class NormalCalculator {

		private Vector3[] points;
		private int[] triangles;

		private int[] lastTriangleUpdateFrame;
		private float[][] triangleNormalCache;

		public NormalCalculator(Vector3[] points, int[] triangles) {
			this.points = points;
			this.triangles = triangles;

			int triangleCount = triangles.Length / 3;

			lastTriangleUpdateFrame = new int[triangleCount];
			triangleNormalCache = new float[triangleCount][];

			for (int i = 0; i < triangleCount; i++) {
				lastTriangleUpdateFrame[i] = -1;
				triangleNormalCache[i] = new float[3];
			}
		}

		public Vector3 GetNormal(Vertex vertex) {
			float x = 0f;
			float y = 0f;
			float z = 0f;

			foreach (int faceId in vertex.Faces) {
				float[] faceNormal = GetTriangleNormal(faceId);
				x += faceNormal[0];
				y += faceNormal[1];
				z += faceNormal[2];
			}

			float mag = Mathf.Sqrt(x * x + y * y + z * z);

			return new Vector3(
				x / mag,
				y / mag,
				z / mag
			);
		}

		private float[] GetTriangleNormal(int faceId) {
			int frameCount = Time.frameCount;

			if (lastTriangleUpdateFrame[faceId] != frameCount) {
				int id = 3 * faceId;
				var p0 = points[triangles[id]];
				var p1 = points[triangles[id + 1]];
				var p2 = points[triangles[id + 2]];

				float ax = p1.x - p0.x;
				float ay = p1.y - p0.y;
				float az = p1.z - p0.z;

				float bx = p2.x - p0.x;
				float by = p2.y - p0.y;
				float bz = p2.z - p0.z;

				float cx = ay * bz - az * by;
				float cy = az * bx - ax * bz;
				float cz = ax * by - ay * bx;

				float mag = Mathf.Sqrt(cx * cx + cy * cy + cz * cz);

				var normal = triangleNormalCache[faceId];
				normal[0] = cx / mag;
				normal[1] = cy / mag;
				normal[2] = cz / mag;

				lastTriangleUpdateFrame[faceId] = frameCount;
			}

			return triangleNormalCache[faceId];
		}

	}

}