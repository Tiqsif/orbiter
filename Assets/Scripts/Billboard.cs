using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;
    void Start()
    {
        mainCamera = Camera.main;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.forward);
            // Adjust the rotation so the quad faces the camera directly (not edge-on)
            //transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        }
    }
}
