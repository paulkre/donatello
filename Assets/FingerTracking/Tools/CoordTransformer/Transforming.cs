using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transforming : MonoBehaviour
{
    public GameObject tester;

    List<Vector3> p0s = new List<Vector3>();
    List<Vector3> p1s = new List<Vector3>();
    List<Vector3> p2s = new List<Vector3>();

    public float noise;

    public Matrix4x4 m44;
    Plotter p;
    // Start is called before the first frame update
    void Start()
    {
        tester = new GameObject("tester");

        for(int i = 0;i<4;i++)
        {
            GameObject g = new GameObject("test" + i);
            g.transform.position = RandomVector(5);
            g.transform.parent = transform;
        }

        foreach(Transform t in transform)
        {
            print(t.gameObject.name + " " + t.transform.position + " " + t.transform.localPosition);
            p0s.Add(t.transform.position);
            p1s.Add(t.transform.localPosition + RandomVector(noise));
            p2s.Add(t.transform.localPosition);
        }

        m44 = transform.worldToLocalMatrix;

        CalculateTransform();

        p = new Plotter(1000, 200, Color.black, 128);
        p.RegisterNewLine();
        p.RegisterNewLine();
    }

    Vector3 RandomVector(float f)
    {
        return new Vector3(Random.Range(-f, f), Random.Range(-f, f), Random.Range(-f, f)); 
    }

    public float[,] m;
    void CalculateTransform()
    {
        float[,] ls = new float[p0s.Count * 3, 12];
        float[,] rs = new float[p0s.Count * 3, 1];

        for (int i = 0;i<p0s.Count;i++)
        {
            for(int j = 0;j<3;j++)
            {
                ls[3 * i +j,4*j+0] = p0s[i].x;
                ls[3 * i +j,4*j+1] = p0s[i].y;
                ls[3 * i +j,4*j+2] = p0s[i].z;
                ls[3 * i +j,4*j+3] = 1;
            }

            rs[3 * i,0] = p1s[i].x;
            rs[3 * i + 1,0] = p1s[i].y;
            rs[3 * i + 2,0] = p1s[i].z;
        }
        //PrintArray(ls);
        //print("//////////////////");
        //PrintArray(rs);
        m = Accord.Math.Matrix.Solve(ls, rs, true);
        //print("//////////////////");
        //PrintArray(m);

        
    }

    private Vector3 ApplyIt(Vector3 v)
    {
        //Vector3 v = tester.transform.position;

        float x1 = m[0, 0] * v.x + m[1, 0] * v.y + m[2, 0] * v.z + m[3, 0];
        float y1 = m[4, 0] * v.x + m[5, 0] * v.y + m[6, 0] * v.z + m[7, 0];
        float z1 = m[8, 0] * v.x + m[9, 0] * v.y + m[10, 0] * v.z + m[11, 0];

        //print($"x:{x1} y:{y1} z:{z1}");

        return new Vector3(x1, y1, z1);
    }

    private Vector3 ApplyItTester()
    {
        Vector3 v = tester.transform.position;

        float x1 = m[0, 0] * v.x + m[1, 0] * v.y + m[2, 0] * v.z + m[3, 0];
        float y1 = m[4, 0] * v.x + m[5, 0] * v.y + m[6, 0] * v.z + m[7, 0];
        float z1 = m[8, 0] * v.x + m[9, 0] * v.y + m[10, 0] * v.z + m[11, 0];

        //print($"x:{x1} y:{y1} z:{z1}");

        return new Vector3(x1, y1, z1);
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

    float error;
    void CalcError(float t)
    {
        error = (1f - t) * error + 
            t * Vector3.Distance(
                transform.InverseTransformPoint(tester.transform.position), 
                ApplyItTester());
    }

    // Update is called once per frame
    void Update()
    {
        tester.transform.position = RandomVector(5);
        CalcError(0.01f);
        print(error);
        p.AddDataLine(error*3f, Color.red, 0);
        p.UpdateImage();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 r = RandomVector(5f);
            p0s.Add(r);
            p1s.Add(transform.InverseTransformPoint(r) + RandomVector(noise));
            p2s.Add(transform.InverseTransformPoint(r));
            CalculateTransform();

        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            string sm = "";
            for(int i = 0;i<12;i++)
            {
                for(int j = 0;j<1;j++)
                {
                    sm += m[i, 0];
                }
                sm += "\n";
            }
            Debug.LogWarning(sm);
        }

        for (int i = 0;i<p0s.Count;i++)
        {
            float d = Vector3.Distance(p2s[i], ApplyIt(p0s[i]));
            Debug.DrawLine(p2s[i], ApplyIt(p0s[i]), Tools.Colors.RedYellowRed(1f-d/noise));
        }

        if(Time.time>5 && Random.Range(0f,1f)<1.1f)
        {
            for(int i = 0;i<5;i++)
            {
                Vector3 r = RandomVector(5f);
                p0s.Add(r);
                p1s.Add(transform.InverseTransformPoint(r) + RandomVector(noise));
                p2s.Add(transform.InverseTransformPoint(r));
            }

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Restart();

            CalculateTransform();

            print("calc Time " + p0s.Count + " " + sw.ElapsedTicks / 10000f);

        }
    }

    Rect errorBox = new Rect(5, 5, 200, 30);
    void OnGUI()
    {
        errorBox.width = error * 5000;
        GUI.backgroundColor = Color.red;
        GUI.Box(errorBox, ""+p0s.Count);
        GUI.DrawTexture(new Rect(5, 50, 1000, 200), p.image);
    }

}
