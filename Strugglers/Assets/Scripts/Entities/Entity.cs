using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Entity : MonoBehaviour
{
    public int life;
    public bool isDead;

    // Start is called before the first frame update
    public void init()
    {
        isDead = false;
        life = Random.Range(2000, 2500);
    }

    // Update is called once per frame
    public void updateLife()
    {
        life--;
        if (life <= 0)
        {
            die();
        }
    }

    public void die()
    {
        life = 0;
        isDead = true;
        gameObject.SetActive(false);
    }

    public GameObject reproduce()
    {
        GameObject go = GameObject.Instantiate(transform.gameObject) as GameObject;
        go.transform.localScale = new Vector3(Random.Range(0.2f, 0.3f), Random.Range(0.2f, 0.3f), Random.Range(0.2f, 0.3f));
        go.transform.position = new Vector3(transform.position.x + Random.Range(5f, 10f), transform.position.y + Random.Range(5f, 10f), transform.position.z + Random.Range(5f, 10f));
        return go;
    }
}
