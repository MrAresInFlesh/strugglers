/**
 * Auteurs 	: 	Simon Meier, Yasmine Margueron, Simon Porret
 * Date		: 	27.08.2021
 * Version	: 	2.1.2
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{

    public Camera cameraDisplay1;
    public Camera cameraDisplay2;
    public Camera cameraDisplay3;

    // Start is called before the first frame update
    void Start()
    {
        cameraDisplay1.enabled = true;
        cameraDisplay2.enabled = false;
        cameraDisplay3.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        switchDisplay();
    }

    private void switchDisplay()
    {
        if (Input.GetKeyDown("1"))
        {
            cameraDisplay1.enabled = true;
            cameraDisplay2.enabled = false;
            cameraDisplay3.enabled = false;
        }
        if (Input.GetKeyDown("2"))
        {
            cameraDisplay1.enabled = false;
            cameraDisplay2.enabled = true;
            cameraDisplay3.enabled = false;
        }
        if (Input.GetKeyDown("3"))
        {
            cameraDisplay1.enabled = false;
            cameraDisplay2.enabled = false;
            cameraDisplay3.enabled = true;
        }
    }
}
