using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileIndicator : MonoBehaviour
{
    // mesh 0
    public Mesh mesh0; // transparent mesh
    public Material material0; // transparent meshes material
    public MeshRenderer meshRenderer0;
    public MeshFilter meshFilter0;

    // mesh 1
    public Mesh mesh1; // opaque mesh
    public Material material1; // opaque meshes material
    public MeshRenderer meshRenderer1;
    public MeshFilter meshFilter1;

    public int segments = 2;

    private float angle;
    private float width;

    private ArcManager arcManager;
    public void Create(ArcManager arcManager, Vector3 direction, float width)
    {
        this.arcManager = arcManager;
        this.width = width;

        // create two meshes, one transparent and one opaque
        // transparent mesh will stretch from the center to the orbs end position
        // opaque mesh will stretch from the center to the orbs current position

        mesh0 = new Mesh();
        GameObject indicator0 = new GameObject("Indicator0");
        indicator0.layer = LayerMask.NameToLayer("Indicator");
        indicator0.tag = "Indicator";
        indicator0.transform.parent = this.transform;
        meshFilter0 = indicator0.AddComponent<MeshFilter>();
        meshRenderer0 = indicator0.AddComponent<MeshRenderer>();
        meshFilter0.mesh = mesh0;

        mesh1 = new Mesh();
        GameObject indicator1 = new GameObject("Indicator1");
        indicator1.layer = LayerMask.NameToLayer("Indicator");
        indicator1.tag = "Indicator";
        indicator1.transform.parent = this.transform;
        meshFilter1 = indicator1.AddComponent<MeshFilter>();
        meshRenderer1 = indicator1.AddComponent<MeshRenderer>();
        meshFilter1.mesh = mesh1;

        indicator1.transform.position = indicator0.transform.position + new Vector3(0, 0.5f, 0);

        // caclulate angle from just direction
        angle = Mathf.Atan2(direction.normalized.z, direction.normalized.x) * Mathf.Rad2Deg;
        

        // ------------------------------------- MESH 0 -----------------------------------------------------

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];


        vertices[0] = arcManager.transform.position; // circles center

        float radius = arcManager.circleRadius;
        float len = Mathf.Rad2Deg * width / radius; // angle of the arc
        float angleStep = len / segments; // angle between each vertex
        float startAngle = angle - len / 2;

        // loop to create vertices along the arc
        for (int i = 0; i <= segments; i++)
        {
            float angle = startAngle + angleStep * i;
            float radian = angle * Mathf.Deg2Rad;

            vertices[i + 1] = arcManager.transform.position + new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian)) * radius;

        }

        // loop to create triangles
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 2;
            triangles[i * 3 + 2] = i + 1;
        }

        // Set mesh properties
        mesh0.vertices = vertices;
        mesh0.triangles = triangles;
        mesh0.RecalculateNormals();

        meshFilter0.mesh = mesh0;

        meshRenderer0.material = material0;

        


    }
    

    public void UpdateMesh(float ratio)
    {
        // ------------------------------------- MESH 1 -----------------------------------------------------
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

       
        vertices[0] = arcManager.transform.position; // circles center

        float radius = arcManager.circleRadius;
        float len = Mathf.Rad2Deg * width / (radius * ratio); // angle of the arc
        float angleStep = len / segments; // angle between each vertex
        float startAngle = angle - len/2;

        // loop to create vertices along the arc
        for (int i = 0; i <= segments; i++)
        {
            float angle = startAngle + angleStep * i;
            float radian = angle * Mathf.Deg2Rad;

            vertices[i + 1] = arcManager.transform.position + new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian)) * radius * ratio;

        }

        // loop to create triangles
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 2;
            triangles[i * 3 + 2] = i + 1;
        }

        mesh1.Clear();
        // Set mesh properties
        mesh1.vertices = vertices;
        mesh1.triangles = triangles;
        mesh1.RecalculateNormals();
        meshFilter1.mesh = mesh1;
        meshRenderer1.material = material1;
    }
}
