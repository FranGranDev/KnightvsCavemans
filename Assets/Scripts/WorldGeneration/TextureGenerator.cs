using UnityEngine;
using System.Collections;

public static class TextureGenerator
{
    public static Texture2D TextureFromHeightMap(float[,] Map, Gradient gradient)
    {
        int xSize = Map.GetLength(0);
        int ySize = 0;
        for(int x = 0; x < xSize; x++)
        {
            if(Map[x, 1] > ySize)
            {
                ySize = Mathf.RoundToInt(Map[x, 1] + 20)
;
            }
        }

        Texture2D texture = new Texture2D(xSize, ySize);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y <= ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                float Evaluate = ((float)y / (float)ySize);
                texture.SetPixel(x, y, gradient.Evaluate(Evaluate));
                Debug.Log(gradient.Evaluate(Evaluate));
            }
        }
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureFromColourMap(Color[,] colourMap, int xSize, int ySize)
    {
        Texture2D texture = new Texture2D(xSize, ySize);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                texture.SetPixel(x, y, colourMap[x, y]);
            }
        }
        texture.Apply();
        return texture;
    }

    public static Texture2D TextureStandart(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }

}
