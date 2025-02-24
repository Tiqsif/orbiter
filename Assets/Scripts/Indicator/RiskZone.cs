using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiskZone : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private int _segments = 5;
    [SerializeField] private float _width = 0.5f;
    [Space(10)]
    public float arcAngle;
    public Vector3 direction;
    
    private float _lifetime;
    private float _radius;

    public void Create(Vector3 dir, float radius, float arcAngle, float lifetime)
    {
        //Invoke("DestroySelf", lifetime); // handled in arcmanager
        this._radius = radius;
        this.direction = dir;
        this._lifetime = lifetime;
        this.arcAngle = arcAngle;

        // calculate segments based on arc angle and radius

        _segments = Mathf.CeilToInt(arcAngle * radius / 100);

        // create mesh. all the vertices are on the circle
        Mesh mesh = new Mesh();
        GameObject indicator = new GameObject("RiskZoneMesh");
        indicator.transform.parent = this.transform;
        MeshFilter meshFilter = indicator.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = indicator.AddComponent<MeshRenderer>();
        //meshFilter.mesh = mesh;
        meshRenderer.material = _material;

        float angle = Mathf.Atan2(direction.normalized.z, direction.normalized.x) * Mathf.Rad2Deg;

        int totalVertices = (_segments + 1) * 2; // Outer and inner vertices
        Vector3[] vertices = new Vector3[totalVertices];
        int[] triangles = new int[_segments * 6]; // Two triangles per segment

        float angleStep = arcAngle / _segments; // Angle between each vertex
        float startAngle = angle - arcAngle / 2;

        indicator.transform.position = transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;

        // Generate vertices for the outer and inner arcs
        for (int i = 0; i <= _segments; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            float radian = currentAngle * Mathf.Deg2Rad;

            // Outer vertex
            vertices[i] = transform.position + new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian)) * (radius + _width / 2) + (transform.position - indicator.transform.position);

            // Inner vertex
            vertices[i + (_segments + 1)] = transform.position + new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian)) * (radius - _width / 2) + (transform.position - indicator.transform.position);
        }

        // Generate triangles
        for (int i = 0; i < _segments; i++)
        {
            // First Triangle (Outer -> Inner -> Next Outer)
            triangles[i * 6] = i;
            triangles[i * 6 + 1] = i + (_segments + 1);
            triangles[i * 6 + 2] = i + 1;

            // Second Triangle (Next Outer -> Inner -> Next Inner)
            triangles[i * 6 + 3] = i + 1;
            triangles[i * 6 + 4] = i + (_segments + 1);
            triangles[i * 6 + 5] = i + (_segments + 2);
        }

        // Set mesh properties
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        meshRenderer.material = this._material;


        Debug.DrawLine(transform.position, transform.position + direction.normalized * 12, Color.magenta, 5f);

        // tween scale up from 0
        Vector3 originalScale = indicator.transform.localScale;
        indicator.transform.localScale = Vector3.zero;
        indicator.transform.DOScale(originalScale, 0.8f).SetEase(Ease.OutBounce);
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
