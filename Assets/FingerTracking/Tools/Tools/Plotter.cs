using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plotter
{
    int sizeX, sizeY;
    PixelUpdate[] pixelUpdates;
    public int updates;
    List<float>[] pixelList;
    int currentPosition;

    private Color32[] whitePixels;
    private Color background;

    public Texture2D image;

    public List<Line> lines;

    public Plotter(int sizeX, int sizeY, Color background, int buffer)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;

        pixelList = new List<float>[sizeX];
        for(int i = 0;i<pixelList.GetLength(0);i++)
        {
            pixelList[i] = new List<float>(16);
        }

        this.background = background;
        image = new Texture2D(sizeX, sizeY, TextureFormat.RGB24, false);
        whitePixels = new Color32[sizeX * sizeY];
        for(int i = 0;i<whitePixels.Length;i++)
        {
            whitePixels[i] = new Color32((byte)(255*background.r), (byte)(255 * background.g), (byte)(255 * background.b), (byte)(255 * background.a));
        }
        image.SetPixels32(whitePixels);
        image.Apply();

        currentPosition = 0;

        SetBufferSize(buffer);

        lines = new List<Line>(8);
    }

    public void SetBufferSize(int buffer)
    {
        pixelUpdates = new PixelUpdate[buffer];
        for (int i = 0; i < buffer; i++)
        {
            pixelUpdates[i] = new PixelUpdate();
        }
        updates = 0;
    }

    public int RegisterNewLine()
    {
        lines.Add(new Line());
        return lines.Count - 1;
    }

    public void AddDataLine(float v0, float v1, Color c)
    {
        if (currentPosition < sizeX - 1)
        {
            //DrawLine(currentPosition, v0, v1, c);
            MoveToNextPosition();
        }
        else
        {
            MoveToNextPosition();
            DrawPixel(currentPosition, v1, c);
            MoveToNextPosition();
        }
    }

    public void AddDataLine(float v, Color c, int lineIndex)
    {
        int target = Mathf.RoundToInt(Mathf.Clamp01(v) * (sizeY - 1));
        int step = target > lines[lineIndex].lastValue ? 1 : -1;
        int steps = Mathf.Abs(target - lines[lineIndex].lastValue);

        DrawPixel(currentPosition, (float)((lines[lineIndex].lastValue)) / (sizeY - 1), c);
        for (int d = 1; d < steps; ++d)
        {
            int r = Mathf.RoundToInt(d / steps);
            DrawPixel(currentPosition + r, (float)((lines[lineIndex].lastValue + d*step))/(sizeY - 1), c);
        }
        lines[lineIndex].lastValue = target;
    }

    public void AddDataPoint(float v, Color c)
    {
        DrawPixel(currentPosition, v, c);
        MoveToNextPosition();
    }

    public void DrawPixel(int x, float v, Color c)
    {
        pixelUpdates[updates].SetData(x, v, c);
        updates++;

        if (updates >= pixelUpdates.Length)
        {
            SetBufferSize(2 * pixelUpdates.Length);
        }
    }
    
    public void UpdateImage()
    {
        byte[] pixels = image.GetRawTextureData();
        for(int i = 0;i<updates;i++)
        {
            //Debug.Log("update: " + pixelUpdates[i].x + " " + pixelUpdates[i].v );
            int y = ValueToInt(pixelUpdates[i].v);
            int index = 3*(pixelUpdates[i].x + sizeX * y);

            pixels[index]   = (byte)(pixelUpdates[i].color.r * 255);
            pixels[index+1] = (byte)(pixelUpdates[i].color.g * 255);
            pixels[index+2] = (byte)(pixelUpdates[i].color.b * 255);

            pixelList[pixelUpdates[i].x].Add(pixelUpdates[i].v);
        }
        image.LoadRawTextureData(pixels);
        image.Apply(false);

        updates = 0;
        MoveToNextPosition();
    }

    private int ValueToInt(float v)
    {
        return Mathf.RoundToInt(Mathf.Clamp01(v) * (sizeY - 1));
    }
	
    private void MoveToNextPosition()
    {
        currentPosition = currentPosition == sizeX-1 ? 0 : ++currentPosition;
        foreach(float f in pixelList[currentPosition])
        {
            DrawPixel(currentPosition, f, background);
        }
        pixelList[currentPosition].Clear();
    }
}

public class PixelUpdate
{
    public int x;
    public float v;
    public Color color;

    public PixelUpdate() { }

    public void SetData(int x, float v, Color color)
    {
        this.x = x;
        this.v = v;
        this.color = color;
    }
}

public class Line
{
    public int lastValue = 0;
    //public int lastPosition = 0;

    public Line() { }
    
}
