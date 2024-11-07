using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class AttackIndicator : MonoBehaviour
{

    public Mesh mesh;
    public Material material;
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;
    public int segments = 10;
    private float angle;
    private float width;
    private ArcManager arcManager;
    public void Create(ArcManager arcManager, Vector3 direction, float width)
    {
        this.arcManager = arcManager;
        this.width = width;
        mesh = new Mesh();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter.mesh = mesh;
        material ??= new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        material.color = new Color(1, 0, 0, 1f);
        material.SetFloat("_Surface", 1); // 1 for Transparent, 0 for Opaque
        material.SetFloat("_Blend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha); // Enable alpha blending
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        meshRenderer.material = material;

        // caclulate angle from just direction
        angle = Mathf.Atan2(direction.normalized.z, direction.normalized.x) * Mathf.Rad2Deg;

        UpdateMesh(0f);
    }
    

    public void UpdateMesh(float ratio)
    {
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

       
        vertices[0] = arcManager.transform.position; // circles center

        float radius = arcManager.circleRadius;
        float len = Mathf.Rad2Deg * width / radius; // angle of the arc
        float angleStep = len / segments; // angle between each vertex
        float startAngle = angle - len/2;

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

        mesh.Clear();
        // Set mesh properties
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        material.color = new Color(1, 0, 0, ratio);
        Debug.Log(ratio);
        meshRenderer.material = material;
    }
}
