/**
 * Auteurs 	: 	Simon Meier, Yasmine Margueron, Simon Porret
 * Date		: 	08.09.2021
 * Version	: 	4.1.2
 */

/**
 * Static class to manage entity during the main program.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EntityManager
{
    public static List<GameObject> generateEntities(int population, PrimitiveType primitiveType, GameObject gameObject)
    {
        List<GameObject> pop = new List<GameObject>(population);
        
        for (int i = 0; i <= population; i++)
        {
            pop[i].AddComponent<Rigidbody>();
            pop[i].GetComponent<Renderer>().sharedMaterial.color = Color.green;
            pop[i].GetComponent<Rigidbody>().isKinematic = true;
            pop[i].AddComponent<Animal>();
            pop[i].AddComponent<FieldOfView>();
        }

        for (int i = 0; i < pop.Count; i++)
        {
            for (int j = 0; j < pop.Count; j++)
            {
                pop[i].transform.position = new Vector3(j + 10, 30, i + 10);
            }
        }

        return pop;
    }

    public static List<GameObject> generateEntitiesFromList(List<GameObject> population, PrimitiveType primitiveType)
    {
        List<GameObject> pop = population;

        for (int i = 0; i <= pop.Count; i++)
        {
            pop.Add(GameObject.CreatePrimitive(primitiveType));
            pop[i].AddComponent<Rigidbody>();
            pop[i].GetComponent<Rigidbody>().useGravity = true;
            pop[i].GetComponent<Renderer>().sharedMaterial.color = Color.cyan;
        }

        for (int i = 0; i < pop.Count; i++)
        {
            for (int j = 0; j < pop.Count; j++)
            {
                pop[i].transform.position = new Vector3(j + 10, 30, i + 10);
            }
        }

        return pop;
    }

    public static void destroyEntities(List<GameObject> entities, bool editMode)
    {
        if (entities.Count != 0)
        {
            foreach (GameObject entity in entities)
            {
                if (editMode)
                {
                    GameObject.DestroyImmediate(entity);
                }
                else
                {
                    entities.Remove(entity);
                    GameObject.Destroy(entity);
                }
            }
            entities.Clear();
        }
        else
        {
            Debug.Log("Entities has no population");
        }
    }

    public static void destroyEntityWhenDead(List<GameObject> entities)
    {
        if (entities.Count != 0)
        {
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                if (entities[i] != null)
                {
                    if (entities[i].GetComponent<Entity>().isDead)
                    {
                        entities[i].GetComponent<Entity>().die();
                        GameObject.Destroy(entities[i]);
                        entities.RemoveAt(i);
                    }
                }
            }
        }
    }
}
