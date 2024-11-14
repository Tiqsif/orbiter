using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircle : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float radius;
    public int segments;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float angleStep = 2 * Mathf.PI / segments;
        lineRenderer.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Sin(angleStep * i) * radius;
            float z = Mathf.Cos(angleStep * i) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
        }
        
    }
}
