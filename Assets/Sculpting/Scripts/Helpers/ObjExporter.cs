using System.IO;
using System.Text;
using System.Globalization;
using UnityEngine;

namespace VRSculpting.Helpers
{

    public static class ObjExporter
    {

        public static void Export(Mesh mesh, Transform transform, string filename = null)
        {
            if (filename == null)
                filename = $"out_{System.DateTime.Now.ToString("yyMMdd-HHmmss")}.obj";
            string objString = MeshToString(mesh, transform);
            WriteToFile(objString, filename);
        }

        private static string MeshToString(Mesh mesh, Transform transform)
        {
            Quaternion rotation = transform.localRotation;

            var sb = new StringBuilder();

            foreach (Vector3 p in mesh.vertices)
            {
                Vector3 tp = transform.TransformPoint(p);
                sb.Append("v ");
                sb.Append(PointToString(tp));
                sb.Append("\n");
            }

            sb.Append("\n");

            foreach (Vector3 n in mesh.normals)
            {
                Vector3 tn = rotation * n;
                sb.Append("vn ");
                sb.Append(PointToString(tn));
                sb.Append("\n");
            }

            sb.Append("\n");

            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                sb.Append(string.Format("f {0} {1} {2}\n",
                    triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
            }

            return sb.ToString();
        }

        private static string PointToString(Vector3 p)
        {
            return string.Format(
                "{0} {1} {2}",
                p.x.ToString(CultureInfo.InvariantCulture),
                p.y.ToString(CultureInfo.InvariantCulture),
                p.z.ToString(CultureInfo.InvariantCulture)
            );
        }

        private static void WriteToFile(string data, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(data);
            }
        }

    }

}

