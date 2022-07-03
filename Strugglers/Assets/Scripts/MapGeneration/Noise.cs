using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// no real need of instance
// -> that's why it is static
/**
 * 
 */

public static class Noise
{
    public enum NormalizeMode { Local, Global }

    public static float[,] generateNoiseMap(int width, int height, int seed, float scale, int octave, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
    {
        float[,] noiseMap = new float[width, height];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octave];

        float maxPossibleHeight = 0;
        float frequency = 1, amplitude = 1, noiseHeight = 0;

        for (int i = 0; i < octave; i++)
        {
            float offset_x = prng.Next(-100000, 100000) + offset.x;
            float offset_y = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offset_x, offset_y);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0) scale = 0.0001f;

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float centerX = width / 2f;
        float centerY = height / 2f;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                frequency = 1; 
                amplitude = 1; 
                noiseHeight = 0;

                for (int k = 0; k < octave; k++)
                {
                    float x = ((j - centerX + octaveOffsets[k].x) / scale * frequency);
                    float y = ((i - centerY + octaveOffsets[k].y) / scale * frequency);

                    float perlinValue = Mathf.PerlinNoise(x, y) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight) maxLocalNoiseHeight = noiseHeight;
                else if (noiseHeight < minLocalNoiseHeight) minLocalNoiseHeight = noiseHeight;

                noiseMap[j, i] = noiseHeight;

            }
        }

        // need to transform the value of noiseMap between 0 and 1
        // for this we normalize using two for loops and changing every value of the noise map.
        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                if (normalizeMode == NormalizeMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (2f * maxPossibleHeight / 1.75f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        return noiseMap;
    }
}
