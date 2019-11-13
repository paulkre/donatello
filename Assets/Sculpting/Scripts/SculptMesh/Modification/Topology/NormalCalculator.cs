using UnityEngine;

namespace VRSculpting.SculptMesh.Modification.Topology
{

    public class NormalCalculator
    {

        private Vertex[] vertices;
        private Vector3[] points;
        private int[] ids;

        private int[] lastTriangleUpdateFrame;
        private float[][] triangleNormalCache;

        public NormalCalculator(Vertex[] vertices, Vector3[] points, int[] ids)
        {
            this.vertices = vertices;
            this.points = points;
            this.ids = ids;

            int triangleCount = ids.Length / 3;

            lastTriangleUpdateFrame = new int[triangleCount];
            triangleNormalCache = new float[triangleCount][];

            for (int i = 0; i < triangleCount; i++)
            {
                lastTriangleUpdateFrame[i] = -1;
                triangleNormalCache[i] = new float[3];
            }
        }

        public bool UpdateNormals(Vector3[] normals, bool[] mask)
        {
            bool didUpdate = false;

            for (int i = 0; i < mask.Length; i++)
            {
                if (mask[i])
                {
                    mask[i] = false;
                    normals[i] = GetNormal(vertices[i]);
                    didUpdate = true;
                }
            }

            return didUpdate;
        }

        private Vector3 GetNormal(Vertex vertex)
        {
            float x = 0f;
            float y = 0f;
            float z = 0f;

            foreach (int faceId in vertex.Faces)
            {
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

        private float[] GetTriangleNormal(int faceId)
        {
            int frameCount = SculptManager.FrameCount;

            if (lastTriangleUpdateFrame[faceId] != frameCount)
            {
                int id = 3 * faceId;
                var p0 = points[ids[id]];
                var p1 = points[ids[id + 1]];
                var p2 = points[ids[id + 2]];

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