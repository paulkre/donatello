using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxProjection : MonoBehaviour
{
    public Material mat;

    const int RESOLUTIONX = 180;
    const int RESOLUTIONY = 120;

    GameObject projectionMesh;
    // Start is called before the first frame update
    void Start()
    {
        projectionMesh = new GameObject("projectionMesh");
        projectionMesh.AddComponent<MeshRenderer>();
        projectionMesh.AddComponent<MeshFilter>();
        Mesh mesh = BuildMesh();
        projectionMesh.GetComponent<MeshFilter>().mesh = mesh;
        projectionMesh.GetComponent<MeshRenderer>().material = mat;
        projectionMesh.AddComponent<MeshCollider>();

    }

    float pX = 0, pY = 0;
    // Update is called once per frame
    void Update()
    {
        SetPoints();
        Solver3Dto2D solver = GetComponent<Solver3Dto2D>();


        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit rch;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rch))
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.position = rch.point;
                solver.Calculate2Dfrom3D(rch.point, ref pX, ref pY);
            }
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            solver.ClearData();

            solver.AddPointCorrespondence(new Vector3(0, EasyNoise(0, 0), 0), points2d[0].x, points2d[0].y);
            solver.AddPointCorrespondence(new Vector3(RESOLUTIONX / 2, EasyNoise(RESOLUTIONX / 2, 0), 0), points2d[1].x, points2d[1].y);
            solver.AddPointCorrespondence(new Vector3(RESOLUTIONX, EasyNoise(RESOLUTIONX, 0), 0), points2d[2].x, points2d[2].y);

            solver.AddPointCorrespondence(new Vector3(0, EasyNoise(0, RESOLUTIONY / 2), RESOLUTIONY / 2), points2d[3].x, points2d[3].y);
            solver.AddPointCorrespondence(new Vector3(RESOLUTIONX / 2, EasyNoise(RESOLUTIONX / 2, RESOLUTIONY / 2), RESOLUTIONY / 2), points2d[4].x, points2d[4].y);
            solver.AddPointCorrespondence(new Vector3(RESOLUTIONX, EasyNoise(RESOLUTIONX, RESOLUTIONY / 2), RESOLUTIONY / 2), points2d[5].x, points2d[5].y);

            solver.AddPointCorrespondence(new Vector3(0, EasyNoise(0, RESOLUTIONY), RESOLUTIONY), points2d[6].x, points2d[6].y);
            solver.AddPointCorrespondence(new Vector3(RESOLUTIONX / 2, EasyNoise(RESOLUTIONX / 2, RESOLUTIONY), RESOLUTIONY), points2d[7].x, points2d[7].y);
            solver.AddPointCorrespondence(new Vector3(RESOLUTIONX, EasyNoise(RESOLUTIONX, RESOLUTIONY), RESOLUTIONY), points2d[8].x, points2d[8].y);

            solver.AddPointCorrespondence(new Vector3(RESOLUTIONX / 4, EasyNoise(RESOLUTIONX / 4, RESOLUTIONY / 4), RESOLUTIONY / 4), points2d[9].x, points2d[9].y);
            solver.AddPointCorrespondence(new Vector3(3* RESOLUTIONX / 4, EasyNoise(3 * RESOLUTIONX / 4, RESOLUTIONY / 4), RESOLUTIONY / 4), points2d[10].x, points2d[10].y);
            solver.AddPointCorrespondence(new Vector3(RESOLUTIONX / 4, EasyNoise(RESOLUTIONX / 4, 3 * RESOLUTIONY / 4), 3 * RESOLUTIONY / 4), points2d[11].x, points2d[11].y);
            solver.AddPointCorrespondence(new Vector3(3 * RESOLUTIONX / 4, EasyNoise(3* RESOLUTIONX / 4, 3* RESOLUTIONY / 4), 3 * RESOLUTIONY / 4), points2d[12].x, points2d[12].y);


            solver.Calculate();
        }
    }

    public Vector2[] points2d = new Vector2[13];
    void SetPoints()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            points2d[0] = Input.mousePosition;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            points2d[1] = Input.mousePosition;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            points2d[2] = Input.mousePosition;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            points2d[3] = Input.mousePosition;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            points2d[4] = Input.mousePosition;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            points2d[5] = Input.mousePosition;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            points2d[6] = Input.mousePosition;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            points2d[7] = Input.mousePosition;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            points2d[8] = Input.mousePosition;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            points2d[9] = Input.mousePosition;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            points2d[10] = Input.mousePosition;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            points2d[11] = Input.mousePosition;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            points2d[12] = Input.mousePosition;
        }
    }

    Mesh BuildMesh()
    {
        Mesh m = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvcoords = new List<Vector2>();

        for (int y = 0; y < RESOLUTIONY; y++)
        {
            for (int x = 0; x < RESOLUTIONX; x++)
            {
                vertices.Add(new Vector3(x, EasyNoise(x,y), y));
                uvcoords.Add(new Vector2((float)x / (RESOLUTIONX - 1), (float) y/ (RESOLUTIONY - 1)));
            }
        }
        
        for (int y = 0; y < RESOLUTIONY-1; y++)
        {
            for (int x = 0; x < RESOLUTIONX - 1; x++)
            {
                triangles.Add(x + y * RESOLUTIONX);
                triangles.Add(x + 1 + (y+1) * RESOLUTIONX);
                triangles.Add(x + 1 + y * RESOLUTIONX);

                triangles.Add(x + y * RESOLUTIONX);
                triangles.Add(x + (y + 1) * RESOLUTIONX);
                triangles.Add(x + 1 + (y+1) * RESOLUTIONX);
            }
        }

        m.vertices = vertices.ToArray();
        m.triangles = triangles.ToArray();
        m.uv = uvcoords.ToArray();
        return m;
    }

    float EasyNoise(int x, int y)
    {

        return 20*Mathf.PerlinNoise(x/100f, y/100f) + 10 * Mathf.PerlinNoise(x / 10f, y / 10f);
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(pX - 2, Screen.height - pY - 2, 5, 5), "");
    }
}
