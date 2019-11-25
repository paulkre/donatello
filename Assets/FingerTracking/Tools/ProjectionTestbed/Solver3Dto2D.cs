using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Math;

// Math (camera porjection) taken from 
// https://www.cc.gatech.edu/classes/AY2016/cs4476_fall/results/proj3/html/xxiong8/index.html
//
public class Solver3Dto2D : MonoBehaviour
{
    private Texture2D debug;

    private GameObject test;
    private List<UnityEngine.Vector3> position3D;
    private List<UnityEngine.Vector2> position2D;

    private float[,] m;
    private float x2d, y2d;

    private const bool ttd = false;
    private const int ttdCount = 10;

    // Start is called before the first frame update
    void Start()
    {
        if(ttd)
        {
            debug = new Texture2D(1, 1);
            debug.SetPixel(0, 0, Color.white);
            debug.Apply();

            test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            test.transform.localScale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f);
        }

        position3D = new List<UnityEngine.Vector3>();
        position2D = new List<Vector2>();

        m = new float[11,1];
    }

    // Update is called once per frame
    void Update()
    {
        if (ttd) TTD();
    }

    public void AddPointCorrespondence(UnityEngine.Vector3 pos3D, float x, float y)
    {
        position3D.Add(pos3D);
        position2D.Add(new Vector2(x, y));
    }

    public void Calculate2Dfrom3D(UnityEngine.Vector3 pos3D, ref float x, ref float y)
    {
        float s = m[8, 0] * pos3D.x + m[9, 0] * pos3D.y + m[10, 0] * pos3D.z + 1;
        x = (m[0, 0] * pos3D.x + m[1, 0] * pos3D.y + m[2, 0] * pos3D.z + m[3, 0]) / s;
        y = (m[4, 0] * pos3D.x + m[5, 0] * pos3D.y + m[6, 0] * pos3D.z + m[7, 0]) / s;
    }

    public void ClearData()
    {
        position2D.Clear();
        position3D.Clear();
    }
    
    public void Calculate()
    {
        int count = position3D.Count;
        float[,] ls = new float[2 * count, 11];
        float[,] rs = new float[2 * count, 1];

        float x, y, z, u, v;
        for (int i = 0; i < count; i++)
        {
            x = position3D[i].x;
            y = position3D[i].y;
            z = position3D[i].z;

            u = position2D[i].x;
            v = position2D[i].y;

            print($"{x} {y} {z} {u} {v} ");

            int j = 2 * i;

            ls[j, 0] = x;
            ls[j, 1] = y;
            ls[j, 2] = z;
            ls[j, 3] = 1;
            ls[j, 4] = 0;
            ls[j, 5] = 0;
            ls[j, 6] = 0;
            ls[j, 7] = 0;
            ls[j, 8] = -u * x;
            ls[j, 9] = -u * y;
            ls[j, 10] = -u * z;
            rs[j, 0] = u;

            j += 1;
            ls[j, 0] = 0;
            ls[j, 1] = 0;
            ls[j, 2] = 0;
            ls[j, 3] = 0;
            ls[j, 4] = x;
            ls[j, 5] = y;
            ls[j, 6] = z;
            ls[j, 7] = 1;
            ls[j, 8] = -v * x;
            ls[j, 9] = -v * y;
            ls[j, 10] = -v * z;
            rs[j, 0] = v;
        }

        m = Matrix.Solve(ls, rs, false);
        PrintArray(ls);
        PrintArray(rs);
        PrintArray(m);
    }


    private void PrintArray(float[,] array)
    {
        for (int x = 0; x < array.GetLength(0); x++)
        {
            for (int y = 0; y < array.GetLength(1); y++)
            {
                print($"x:{x} y:{y} v:{array[x, y]}");
            }
        }
    }

    private void OnGUI()
    {
        if(ttd)
        {
            GUI.color = Color.red;
            GUI.DrawTexture(new Rect(x2d - 5, Screen.height - y2d - 5, 11, 11), debug);

            GUI.color = Color.green;
            float x = Camera.main.WorldToScreenPoint(test.transform.position).x;
            float y = Camera.main.WorldToScreenPoint(test.transform.position).y;
            GUI.DrawTexture(new Rect(x - 2, Screen.height - y - 2, 5, 5), debug);

            GUI.color = Color.white;
            GUI.Label(new Rect(5, 5, 100, 60), $"{x2d}\n{y2d}");
        }
    }

    private void TTD()
    {
        if (position3D.Count < ttdCount)
        {
            UnityEngine.Vector3 a = new UnityEngine.Vector3(Random.Range(-10f, 10f), Random.Range(0f, 30f), Random.Range(40f, 60f));
            test.transform.position = a;
            AddPointCorrespondence(
                a,
                Camera.main.WorldToScreenPoint(a).x,
                Camera.main.WorldToScreenPoint(a).y);

            print($"added ({position2D.Count}): {a.x}, {a.y}, {a.z} " +
                $"-> {Camera.main.WorldToScreenPoint(a).x}, {Camera.main.WorldToScreenPoint(a).y} ");
            if (position3D.Count == ttdCount - 1)
            {
                //SolveProjection();
                Calculate();
            }
        }
        else
        {
            Calculate2Dfrom3D(test.transform.position, ref x2d, ref y2d);
        }
    }
}
