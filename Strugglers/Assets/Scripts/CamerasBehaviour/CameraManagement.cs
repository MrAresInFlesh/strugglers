/**
 * Auteurs 	: 	Simon Meier, Yasmine Margueron, Simon Porret
 * Date		: 	08.09.2021
 * Version	: 	4.1.2
 */
 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManagement : MonoBehaviour
{
    public GameObject character;

    [SerializeField]
    public float sensitivity = 5.0f;
    private float smoothing = 2.0f;

    private Vector2 mouseLook;
    private Vector2 smoothV;

    public bool activeEye;

    private void Start()
    {
        activeEye = true;
    }

    void Update()
    {
        lockVision(activeEye);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (activeEye) activeEye = false;
            else activeEye = true;
        }
    }

    private void lockVision(bool activeEye)
    {
        if (activeEye)
        {
            var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
            smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
            smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
            mouseLook += smoothV;

            transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
            character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
            Cursor.visible = false;
        }
        else if (!activeEye)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


    void OnGUI()
    {

    }
}

