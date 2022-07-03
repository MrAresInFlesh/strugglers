/**
 * Auteurs 	: 	Simon Meier, Yasmine Margueron, Simon Porret
 * Date		: 	29.08.2021
 * Version	: 	1.0.0
 */

/**
 * Static class to spawn a list of GameObject into any kind of generated map using
 * the mesh data informations when loading.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpawnEntities
{
    public static void generateSpawnFor2DMap(MeshData meshData, List<GameObject> entities)
    {
        for (int i = 0; i < entities.Count; i++)
        {
            Vector3 position = meshData.spawnPositions[Random.Range(0, meshData.spawnPositions.Count)];
            entities[i].transform.position = new Vector3(position.x, 1, position.z);
        }
    }
    public static void generateSpawnFor3DMapVegetation(MeshData meshData, List<GameObject> entities, float heightOffset)
    {
        for (int i = 0; i < entities.Count; i ++)
        {
            Vector3 position = meshData.spawnPositions[Random.Range(0, meshData.spawnPositions.Count)];
            entities[i].transform.position = new Vector3(position.x, position.y + 0.1f, position.z);
            entities[i].transform.Rotate(new Vector3(0, i, 0), 1);
            entities[i].transform.localScale = new Vector3(Random.Range(0.4f,1f), Random.Range(0.4f, 1f), Random.Range(0.4f, 1f));
        }
    }
    public static void generateSpawnFor3DMapEarthlingAnimal(MeshData meshData, List<GameObject> entities, float heightOffset)
    {
        for (int i = 0; i < entities.Count; i++)
        {
            Vector3 position = meshData.spawnPositions[Random.Range(0, meshData.spawnPositions.Count)];
            entities[i].transform.position = new Vector3(position.x, position.y + 5f, position.z);
        }
    }

    public static void generateSpawnFor3DMapWaterEntities(MeshData meshData, List<GameObject> entities, float heightOffset)
    {
        for (int i = 0; i < entities.Count; i++)
        {
            Vector3 position = meshData.waterPositions[Random.Range(0, meshData.waterPositions.Count)];
            entities[i].transform.position = new Vector3(position.x, position.y + 0.1f, position.z);
        }
    }
}