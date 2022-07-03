using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(MapGenerator))]
public class Map : Editor
{

    Simulation simulation;

    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if(mapGen.autoUpdate)
            {
                mapGen.drawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.drawMapInEditor();
        }

        if (GUILayout.Button("Generate Entities"))
        {
            simulation.generateCarnivorousPopulation();
        }

        if (GUILayout.Button("Spawn Entities In Map"))
        {
            if(mapGen.entities.Count != 0) SpawnEntities.generateSpawnFor3DMapEarthlingAnimal(mapGen.meshData, mapGen.entities, mapGen.meshHeightFactor);
            else
            {
                Debug.Log("MapGen - Entities has no population");
            }
        }

        if (GUILayout.Button("Remove Entities"))
        {
            EntityManager.destroyEntities(mapGen.entities, true);
        }
    }
}
