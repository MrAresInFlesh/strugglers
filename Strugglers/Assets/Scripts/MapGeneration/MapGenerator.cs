/**
 * Auteurs 	: 	Simon Meier, Yasmine Margueron, Simon Porret
 * Date		: 	29.08.2021
 * Version	: 	4.1.2
 */

/**
 * Classe to generate a procedural map.
 */

using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{

    /*____________________|*______________________________*|_____________________*|
    |#####################|*		   VARIABLES          *|#####################*|
    |_____________________|*______________________________*|_____________________*/

    /*--------------------|
    |         ENUM       */

    public enum DrawMode { NoiseMap, ColorMap, Mesh, FalloffMap };
    public DrawMode drawMode;

    public Noise.NormalizeMode normalizeMode;

    public const int mapChunkSize = 241;
    public float noise;

    public int octaves;
    [Range(0, 1)] public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool useFalloff;

    public float meshHeightFactor = 28;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate = false;

    public TerrainType[] terrains;
    public MapData mapData;
    public MeshData meshData;
    
    [Range(1, 200)] public int population = 40;
    public List<GameObject> entities;

    float[,] falloffMap;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    private void Awake()
    {
        falloffMap = FallOffGenerator.generateFallOffMap(mapChunkSize);
    }

    private void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        
        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    public void drawMap()
    {
        mapData = generateMapData(Vector2.zero);
        MapDisplay display = FindObjectOfType<MapDisplay>();
        meshData = MeshGenerator.generateTerrainMesh(mapData.heightMap, meshHeightFactor, meshHeightCurve);
        display.drawMesh(meshData, TextureGenerator.textureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
    }

    public void drawMapInEditor()
    {
        mapData = generateMapData(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap)
        {
            display.drawTexture(TextureGenerator.textureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.drawTexture(TextureGenerator.textureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            meshData = MeshGenerator.generateTerrainMesh(mapData.heightMap, meshHeightFactor, meshHeightCurve);
            display.drawMesh(meshData, TextureGenerator.textureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            display.drawTexture(TextureGenerator.textureFromHeightMap(FallOffGenerator.generateFallOffMap(mapChunkSize)));
        }
    }

    public void requestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            mapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    void mapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData mapData = generateMapData(center);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void requestMeshData (MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            meshDataThread(mapData, lod, callback);
        };

        new Thread(threadStart).Start();
    }

    void meshDataThread (MapData mapData, int lod, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.generateTerrainMesh(mapData.heightMap, meshHeightFactor, meshHeightCurve);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    MapData generateMapData(Vector2 center)
    {
        float[,] noiseMap = Noise.generateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, seed, noise, octaves, persistance, lacunarity, center + offset, normalizeMode);
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                if (useFalloff)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }

                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < terrains.Length; i++)
                {
                    if (currentHeight >= terrains[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = terrains[i].color;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap, colorMap);
    }

    struct MapThreadInfo<T>
    {
        public Action<T> callback;
        public T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}


[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}

public struct MapData
{
    public float[,] heightMap;
    public Color[] colorMap;

    public MapData (float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}