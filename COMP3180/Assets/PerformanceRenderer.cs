using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceRenderer : MonoBehaviour
{
    private Texture2D fpsGraph;
    private float fps = 0f;

    private float[] prevFPS = new float[60];
    private int prevFPSIndex = 0;

    private Color[] textureReset;

    float highestFPS = 0;

    private void Start()
    {
        fpsGraph = new Texture2D(400, 200);
        int size = fpsGraph.width * fpsGraph.height;
        textureReset = new Color[size];

        for (int i = 0; i < size; i++)
        {
            textureReset[i] = new Color(0, 0, 0, 0);
        }
    }

    private void OnGUI()
    {
        // draw FPS
        GUI.Label(new Rect(10, 10, 50, 50), fps.ToString("F2"));
        GUI.DrawTexture(new Rect(60, 10, 100, 50), fpsGraph, ScaleMode.ScaleToFit);
    }

    void Update()
    {
        fps = (1.0f / Time.deltaTime);
        if(fps > highestFPS)
        {
            highestFPS = fps;
        }
        prevFPS[prevFPSIndex++ % prevFPS.Length] = fps;

        // draw fps graph
        float x = 0;

        fpsGraph.SetPixels(textureReset);
        for (int i = 1; i < prevFPS.Length; i++)
        {
            float newX = x + (fpsGraph.width / prevFPS.Length);

            float y = (Mathf.Clamp(prevFPS[(i + prevFPSIndex) % prevFPS.Length], 0, (highestFPS - 1)) / highestFPS) * fpsGraph.height;
            float prevY = (Mathf.Clamp(prevFPS[(i - 1 + prevFPSIndex) % prevFPS.Length], 0, (highestFPS - 1)) / highestFPS) * fpsGraph.height;

            DrawLine(fpsGraph, new Vector2(x, prevY), new Vector2(newX, y), Color.Lerp(Color.red, Color.green, y / fpsGraph.height), 5);
            x = newX;
        }

        fpsGraph.Apply();
    }

    public void DrawLine(Texture2D tex, Vector2 p1, Vector2 p2, Color col, int thickness = 1)
    {
        Vector2 t = p1;
        float frac = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
        float ctr = 0;

        float slope = (p2.y - p1.y) / (p2.x - p1.x);
        float m = -1 / slope;

        while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
        {

            t = Vector2.Lerp(p1, p2, ctr);
            ctr += frac;

            for (int i = -thickness / 2; i < thickness / 2 + 1; i++)
            {
                float x = t.x + i;
                float y = (t.y + i) * m;
                tex.SetPixel((int)x, (int)y, col);
            }
            tex.SetPixel((int)t.x, (int)t.y, col);
        }
    }
}
