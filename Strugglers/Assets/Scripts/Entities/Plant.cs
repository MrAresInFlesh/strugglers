using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Entity
{
    private int reproduceDelay;

    public void Start()
    {
        init();
        reproduceDelay = (int)(Random.value * 700) + 500;
    }

    public void Update()
    {
        if (WorldClock.timer != 0)
        {
            if (!isDead)
            {
                updateLife();
                reproduceDelay--;
                if (reproduceDelay == 0)
                {
                    //reproducePlant();
                    reproduceDelay = (int)(Random.value * 700) + 500;
                }
            }
        }
    }

    private void reproducePlant()
    {
        GameObject go = GameObject.Instantiate(transform.gameObject) as GameObject;
        
        float radius1 = Random.value * 2 + 1;
        int sign1 = (int)(Random.value * 2) - 1;
        if (sign1 == 0) { sign1 = 1; }

        float radius2 = Random.value * 2 + 1;
        int sign2 = (int)(Random.value * 2) - 1;
        if (sign2 == 0) { sign2 = 1; }

        go.transform.position = new Vector3(transform.position.x + sign1 * radius1, transform.position.y, transform.position.z + sign2 * radius2);

    }
}
