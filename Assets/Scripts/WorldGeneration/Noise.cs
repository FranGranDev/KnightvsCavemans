using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int xSize, int Seed, Vector2 Offset, float Scale, float Frequency, float Amplitude)
    {
        float[,] Map = new float[xSize, 2];
        float HalfxSize = xSize * 0.5f;
        System.Random prng = new System.Random(Seed);
        float offsetX = prng.Next(-100000, 100000) + Offset.x;
        float offsetY = prng.Next(-100000, 100000) - Offset.y;
        int Down = 20;

        for (int y = 0; y < 2; y++)
        {
            if (y == 1)
            {
                for (int x = 0; x < xSize; x++)
                {
                    float xSample = (x - HalfxSize + offsetX) / xSize * Frequency * Scale;
                    float Noise = Mathf.PerlinNoise(xSample, 0);
                    Map[x, y] = Noise * Amplitude;
                }
            }
            else
            {
                for (int x = 0; x < xSize; x++)
                {
                    Map[x, y] = -Down;
                }
            }
        }

        return Map;
    }
}