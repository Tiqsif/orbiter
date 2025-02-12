using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileIndicator : MonoBehaviour
{
    // mesh 0
    private Mesh _mesh0; // transparent mesh
    public Material material0; // transparent meshes material
    private MeshRenderer _meshRenderer0;
    private MeshFilter _meshFilter0;

    // mesh 1
    private Mesh _mesh1; // opaque mesh
    public Material material1; // opaque meshes material
    private MeshRenderer _meshRenderer1;
    private MeshFilter _meshFilter1;

    private int _segments = 2;

    private float _angle;
    private float _width;

    private ArcManager _arcManager;
    public void Create(ArcManager arcManager, Vector3 direction, float width)
    {
        this._arcManager = arcManager;
        this._width = width;

        // create two meshes, one transparent and one opaque
        // transparent mesh will stretch from the center to the orbs end position
        // opaque mesh will stretch from the center to the orbs current position

        _mesh0 = new Mesh();
        GameObject indicator0 = new GameObject("Indicator0");
        indicator0.layer = LayerMask.NameToLayer("Indicator");
        indicator0.tag = "Indicator";
        indicator0.transform.parent = this.transform;
        _meshFilter0 = indicator0.AddComponent<MeshFilter>();
        _meshRenderer0 = indicator0.AddComponent<MeshRenderer>();
        _meshFilter0.mesh = _mesh0;

        _mesh1 = new Mesh();
        GameObject indicator1 = new GameObject("Indicator1");
        indicator1.layer = LayerMask.NameToLayer("Indicator");
        indicator1.tag = "Indicator";
        indicator1.transform.parent = this.transform;
        _meshFilter1 = indicator1.AddComponent<MeshFilter>();
        _meshRenderer1 = indicator1.AddComponent<MeshRenderer>();
        _meshFilter1.mesh = _mesh1;

        indicator1.transform.position = indicator0.transform.position + new Vector3(0, 0.5f, 0);

        // caclulate angle from just direction
        _angle = Mathf.Atan2(direction.normalized.z, direction.normalized.x) * Mathf.Rad2Deg;
        

        // ------------------------------------- MESH 0 -----------------------------------------------------

        Vector3[] vertices = new Vector3[_segments + 2];
        int[] triangles = new int[_segments * 3];


        vertices[0] = arcManager.transform.position; // circles center

        float radius = arcManager.circleRadius;
        float len = Mathf.Rad2Deg * width / radius; // angle of the arc
        float angleStep = len / _segments; // angle between each vertex
        float startAngle = _angle - len / 2;

        // loop to create vertices along the arc
        for (int i = 0; i <= _segments; i++)
        {
            float angle = startAngle + angleStep * i;
            float radian = angle * Mathf.Deg2Rad;

            vertices[i + 1] = arcManager.transform.position + new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian)) * radius;

        }

        // loop to create triangles
        for (int i = 0; i < _segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 2;
            triangles[i * 3 + 2] = i + 1;
        }

        // Set mesh properties
        _mesh0.vertices = vertices;
        _mesh0.triangles = triangles;
        _mesh0.RecalculateNormals();

        _meshFilter0.mesh = _mesh0;

        _meshRenderer0.material = material0;

        


    }
    

    public void UpdateMesh(float ratio)
    {
        // ------------------------------------- MESH 1 -----------------------------------------------------
        Vector3[] vertices = new Vector3[_segments + 2];
        int[] triangles = new int[_segments * 3];

       
        vertices[0] = _arcManager.transform.position; // circles center

        float radius = _arcManager.circleRadius;
        float len = Mathf.Rad2Deg * _width / (radius * ratio); // angle of the arc
        float angleStep = len / _segments; // angle between each vertex
        float startAngle = _angle - len/2;

        // loop to create vertices along the arc
        for (int i = 0; i <= _segments; i++)
        {
            float angle = startAngle + angleStep * i;
            float radian = angle * Mathf.Deg2Rad;

            vertices[i + 1] = _arcManager.transform.position + new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian)) * radius * ratio;

        }

        // loop to create triangles
        for (int i = 0; i < _segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 2;
            triangles[i * 3 + 2] = i + 1;
        }

        _mesh1.Clear();
        // Set mesh properties
        _mesh1.vertices = vertices;
        _mesh1.triangles = triangles;
        _mesh1.RecalculateNormals();
        _meshFilter1.mesh = _mesh1;
        _meshRenderer1.material = material1;
    }
}
