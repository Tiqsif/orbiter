using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiskZone : MonoBehaviour
{
    public Material material;
    public int segments = 5;
    public float width = 1;
    public float arcAngle;
    public Vector3 direction;
    private float lifetime;
    private float radius;

    public void Create(Vector3 dir, float radius, float arcAngle, float lifetime)
    {
        //Invoke("DestroySelf", lifetime); // handled in arcmanager
        this.radius = radius;
        this.direction = dir;
        this.lifetime = lifetime;
        this.arcAngle = arcAngle;

        // calculate segments based on arc angle and radius

        segments = Mathf.CeilToInt(arcAngle * radius / 100);

        // create mesh. all the vertices are on the circle
        Mesh mesh = new Mesh();
        GameObject indicator = new GameObject("RiskZoneMesh");
        indicator.transform.parent = this.transform;
        MeshFilter meshFilter = indicator.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = indicator.AddComponent<MeshRenderer>();
        meshFilter.mesh = mesh;
        meshRenderer.material = material;

        float angle = Mathf.Atan2(direction.normalized.z, direction.normalized.x) * Mathf.Rad2Deg;

        int totalVertices = (segments + 1) * 2; // Outer and inner vertices
        Vector3[] vertices = new Vector3[totalVertices];
        int[] triangles = new int[segments * 6]; // Two triangles per segment

        float angleStep = arcAngle / segments; // Angle between each vertex
        float startAngle = angle - arcAngle / 2;

        // Generate vertices for the outer and inner arcs
        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            float radian = currentAngle * Mathf.Deg2Rad;

            // Outer vertex
            vertices[i] = transform.position + new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian)) * (radius + width / 2);

            // Inner vertex
            vertices[i + (segments + 1)] = transform.position + new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian)) * (radius - width / 2);
        }

        // Generate triangles
        for (int i = 0; i < segments; i++)
        {
            // First Triangle (Outer -> Inner -> Next Outer)
            triangles[i * 6] = i;
            triangles[i * 6 + 1] = i + (segments + 1);
            triangles[i * 6 + 2] = i + 1;

            // Second Triangle (Next Outer -> Inner -> Next Inner)
            triangles[i * 6 + 3] = i + 1;
            triangles[i * 6 + 4] = i + (segments + 1);
            triangles[i * 6 + 5] = i + (segments + 2);
        }

        // Set mesh properties
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        meshRenderer.material = this.material;


        Debug.DrawLine(transform.position, transform.position + direction.normalized * 12, Color.magenta, 5f);
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
