/**
 * Auteurs 	: 	Simon Meier, Yasmine Margueron, Simon Porret
 * Date		: 	27.08.2021
 * Version	: 	2.1.2
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEye : MonoBehaviour
{
    public float speed = 10.0f;
    public GameObject eye;
    private float translation;
    private float straffe;

    void Update()
    {
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        straffe = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(straffe, 0, translation);

        if (Input.GetKeyDown("escape"))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
