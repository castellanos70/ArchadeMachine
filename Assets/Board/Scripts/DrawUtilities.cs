﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawUtilies : MonoBehaviour
{
    



public static void drawTriangle(Texture2D texture, Color color, Vector2[] v)
    {
        // at first sort the three vertices by y-coordinate ascending so v1 is the topmost vertice
        sortVerticesAscendingByY(v);

        // here we know that v1.y <= v2.y <= v3.y 
        // check for trivial case of bottom-flat triangle
        if (v[1].y == v[2].y)
        {
              fillBottomFlatTriangle(texture, color, v[0], v[1], v[2]);
        }
        // check for trivial case of top-flat triangle 
        else if (v[0].y == v[1].y)
        {
            fillTopFlatTriangle(texture, color, v[0], v[1], v[2]);
        }
        else
        {
            Vector2 v4 = new Vector2();
            v4.x = (int)(v[0].x + ((float)(v[1].y - v[0].y) / (float)(v[2].y - v[0].y)) * (v[2].x - v[0].x));
            v4.y=v[1].y;

           // Debug.Log("v4=(" + v4.x + ", " + v4.y + ")");

            fillBottomFlatTriangle(texture, color, v[0], v[1], v4);
            fillTopFlatTriangle(texture, color, v[1], v4, v[2]);
        }
    }


    private static void fillBottomFlatTriangle(Texture2D texture, Color color, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        //Debug.Log("fillBottomFlatTriangle   v={ (" + v1.x + "," + v1.y + "), (" +
        //        v2.x + "," + v2.y + "), (" +
        //        v3.x + "," + v3.y + ") }");

        float invslope1 = (v2.x - v1.x) / (v2.y - v1.y);
        float invslope2 = (v3.x - v1.x) / (v3.y - v1.y);

        float curx1 = v1.x;
        float curx2 = v1.x;

        for (int y = (int)v1.y; y <= v2.y; y++)
        {
            drawHorzLine(texture, color, (int)curx1, (int)curx2, y);
            //Debug.Log("    curx1=" + curx1 + ", curx2=" + curx2);
            curx1 += invslope1;
            curx2 += invslope2;
        }
    }


    private static void fillTopFlatTriangle(Texture2D texture, Color color, Vector2 v1, Vector2 v2, Vector2 v3)
    {
        float invslope1 = (v3.x - v1.x) / (v3.y - v1.y);
        float invslope2 = (v3.x - v2.x) / (v3.y - v2.y);

        float curx1 = v3.x;
        float curx2 = v3.x;

        for (int y = (int)v3.y; y > v1.y; y--)
        {
            drawHorzLine(texture, color, (int)curx1, (int)curx2, y);
            curx1 -= invslope1;
            curx2 -= invslope2;
        }
    }

    private static void sortVerticesAscendingByY(Vector2[] v)
    {
        //Debug.Log("sortVerticesAscendingByY(enter)   v={ (" + v[0].x + "," + v[0].y + "), (" +
        //        v[1].x + "," + v[1].y + "), (" +
        //        v[2].x + "," + v[2].y + ") }");

        if (v[2].y < v[1].y)
        {
            swap(v, 1,2);
        }
        if (v[1].y < v[0].y) swap(v, 0, 1);
        if (v[2].y < v[1].y) swap(v, 1, 2); 
    }


    //===================================================================================
    //It is totally unclear to me (joel) when Unity and or C# pass by reference verses by value
    //  For example, this swap, passing an array, works by reference, but
    //  Passing objects: private static void swap(Vector2 v1, Vector2 v2) is passed by value.
    //===================================================================================
    private static void swap(Vector2[] v, int i, int k)
    {
        Vector2 tmp = v[i];
        v[i] = v[k];
        v[k] = tmp;
    }


    private static void drawHorzLine(Texture2D texture, Color color, int x1, int x2, int y)
    {
        if (x1 > x2)
        {
            int tmp = x1;
            x1 = x2;
            x2 = tmp;
        }
        for (int x = x1; x <= x2; x++)
        {
            texture.SetPixel(x, y, color);
        }
    }


    public static void setTextureColor(Texture2D texture, int pixelSize, Color color)
    {
        for (int x = 0; x < pixelSize; x++)
        {
            for (int y = 0; y < pixelSize; y++)
            {
                texture.SetPixel(x, y, Color.black);
            }
        }
    }



    public static void generateKaleidoscopicTexture(Material material, int pixelSize)
    {
        Texture2D texture = new Texture2D(pixelSize, pixelSize, TextureFormat.ARGB32, false);

        setTextureColor(texture, pixelSize, Color.black);

        Vector2[] v = new Vector2[3];
        Vector2[] w = new Vector2[3];
        for (int n = 0; n < 6; n++)
        {
            for (int i = 0; i < v.Length; i++)
            {
                v[i].x = Random.Range(0, 31);
                v[i].y = Random.Range(0, 31);

                // Confine initial pattern to lower right quadrant
                if (v[i].x > v[i].y)
                {
                    float tmp = v[i].x;
                    v[i].x = v[i].y;
                    v[i].y = tmp;
                }
            }
            Color color = Background_DLA_Script.palette[0, Random.Range(0, 6)];

            for (int k = 0; k < 8; k++)
            {
                kaleidoscopicReflect(v, w, k, 32);
                drawTriangle(texture, color, w);
            }
        }
        texture.Apply();
        material.mainTexture = texture;
    }


    private static void kaleidoscopicReflect(Vector2[] v, Vector2[] w, int n, int offset)
    {
        for (int i = 0; i < v.Length; i++)
        {
            if (n == 0)
            {
                w[i].x = v[i].x + offset;
                w[i].y = v[i].y + offset;
            }
            else if (n == 1)
            {
                w[i].x = -v[i].x + offset;
                w[i].y = v[i].y + offset;
            }
            else if (n == 2)
            {
                w[i].x = v[i].x + offset;
                w[i].y = -v[i].y + offset;
            }
            else if (n == 3)
            {
                w[i].x = -v[i].x + offset;
                w[i].y = -v[i].y + offset;
            }
            else if (n == 4)
            {
                w[i].x = v[i].y + offset;
                w[i].y = v[i].x + offset;
            }
            else if (n == 5)
            {
                w[i].x = -v[i].y + offset;
                w[i].y = v[i].x + offset;
            }
            else if (n == 6)
            {
                w[i].x = v[i].y + offset;
                w[i].y = -v[i].x + offset;
            }
            else if (n == 7)
            {
                w[i].x = -v[i].y + offset;
                w[i].y = -v[i].x + offset;
            }
        }
    }




    public static void generatePeriodicNoiseTexture(Material material, int pixelSize)
    {
        material.SetFloat("_Glossiness", 0.0f);
        material.SetFloat("_Metallic", 0.0f);
        Texture2D texture = new Texture2D(pixelSize, pixelSize, TextureFormat.ARGB32, false);


        


        //Color[] palette = {
        //   new Color(0.000f, 0.000f, 0.000f),
        //   new Color(0.118f, 0.118f, 0.118f),
        //    //new Color(0.243f, 0.243f, 0.259f),
        //    new Color(0.188f, 0.420f, 0.290f)
        // };

        Color[] palette = {
           new Color(0.000f, 0.000f, 0.000f),
           new Color(0.349f, 0.173f, 0.000f),
           new Color(0.592f, 0.259f, 0.063f)
           //new Color(1.000f, 0.459f, 0.094f)
        };


        float noiseScale = 500f;

        float x0 = Random.value;
        float y0 = Random.value;

        for (int x = 0; x < pixelSize; x++)
        {
            for (int y = 0; y < pixelSize; y++)
            {
                //(1 + sin((x + noise(x * 5, y * 5) / 2) * 50)) / 2
                //var val = (1 + sin((x + noise(x*noiseScale, y*noiseScale)/2)*50))*128;

                float xCoord = noiseScale*(x0 + (x / pixelSize));
                float yCoord = noiseScale*(y0 + (y / pixelSize));

                float noise = Mathf.PerlinNoise(xCoord, yCoord);

                //float val = (1 + Mathf.Sin((x/pixelSize + noise) * pixelSize/2)) / 2;
                //Debug.Log("noise=" + noise);
                float val = noise;
                //int idx = 0;
                //if (val < 0.3333) idx = 0;
                //else if (val < 0.8627) idx = 1;
                //else idx = 2;
                //texture.SetPixel(x, y, palette[idx]);
                texture.SetPixel(x, y, new Color(val, val, val));
            }
        }

        texture.Apply();
        material.mainTexture = texture;
    }
}
