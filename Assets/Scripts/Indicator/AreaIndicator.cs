using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaIndicator : MonoBehaviour
{
    public AudioClip blipClip;
    // mesh 0
    private Mesh _mesh;
    public Material materialWhite; 
    public Material materialAttack;

    private MeshRenderer _meshRenderer;
    private MeshFilter _meshFilter;

    

    private int _segments = 10;

    private float _angle;
    private float _width;

    private float _pitchChange = 0.2f;
    private int _flashCount = 4;

    private ArcManager _arcManager;
    public void Create(ArcManager arcManager, Vector3 direction, float width)
    {
        this._arcManager = arcManager;
        this._width = width;

        // create mesh. first vertex is the center of the circle, the rest are on the circle

        _mesh = new Mesh();
        GameObject indicator = new GameObject("IndicatorMesh");
        indicator.layer = LayerMask.NameToLayer("Indicator");
        indicator.tag = "Indicator";
        indicator.transform.parent = this.transform;
        _meshFilter = indicator.AddComponent<MeshFilter>();
        _meshRenderer = indicator.AddComponent<MeshRenderer>();
        _meshFilter.mesh = _mesh;


        // caclulate angle from just direction
        _angle = Mathf.Atan2(direction.normalized.z, direction.normalized.x) * Mathf.Rad2Deg;
        

        // ------------------------------------- MESH -----------------------------------------------------

        Vector3[] vertices = new Vector3[_segments + 2];
        int[] triangles = new int[_segments * 3];


        vertices[0] = arcManager.transform.position; // circles center

        float radius = arcManager.circleRadius;
        float len = width; // angle of the arc
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
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();

        _meshFilter.mesh = _mesh;

        _meshRenderer.material = materialWhite;

        


    }
    

    public IEnumerator FlashRoutine(float duration)
    {
        float startAlpha = materialWhite.color.a;
        // flash flashCount times
        for (int i = 0; i < _flashCount; i++)
        {
            yield return StartCoroutine(FadeAlpha(0f, (duration / _flashCount) / 2));
            yield return StartCoroutine(FadeAlpha(startAlpha, (duration / _flashCount) / 2));
        }

        Color finalColor = materialWhite.color;
        finalColor.a = startAlpha;
        materialWhite.color = finalColor;
    }

    private IEnumerator FadeAlpha(float targetAlpha, float duration)
    {
        float startAlpha = materialWhite.color.a;
        float elapsedTime = 0f;
        //Debug.Log("Start Alpha: " + startAlpha + " Target Alpha: " + targetAlpha);

        while (elapsedTime < duration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            Color color = materialWhite.color;
            color.a = newAlpha;
            materialWhite.color = color;
            if (newAlpha == startAlpha && targetAlpha > startAlpha)
            {
                // change blip clips pitch a little higher through time to create a sense of urgency
                AudioManager.Instance.KillSFX(blipClip);
                AudioManager.Instance.PlaySFX(blipClip).pitch += _pitchChange;
                _pitchChange += 0.2f;
            }
            /*
            if (newAlpha > 0.5f)
            {
                materialWhite.EnableKeyword("_EMISSION");
            }
            else
            {
                materialWhite.DisableKeyword("_EMISSION");
            }
            */

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure target alpha is set at the end
        Color finalColor = materialWhite.color;
        finalColor.a = targetAlpha;
        materialWhite.color = finalColor;
        //materialWhite.EnableKeyword("_EMISSION");
    }

    public void HitMode()
    {
        _meshRenderer.material = materialAttack;
        // create lightning particle effect
    }
}
