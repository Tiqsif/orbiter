using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircle : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _radius = 12.5f;
    [SerializeField] private int _segments = 48;
    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float angleStep = 2 * Mathf.PI / _segments;
        _lineRenderer.positionCount = _segments;
        for (int i = 0; i < _segments; i++)
        {
            float x = Mathf.Sin(angleStep * i) * _radius;
            float z = Mathf.Cos(angleStep * i) * _radius;
            _lineRenderer.SetPosition(i, new Vector3(x, 0, z));
        }
        
    }
}
