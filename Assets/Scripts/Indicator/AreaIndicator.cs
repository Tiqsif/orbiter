using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaIndicator : MonoBehaviour
{
    public AudioClip blipClip;
    // mesh 0
    public Mesh mesh;
    public Material materialWhite; 
    public Material materialAttack;

    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    

    public int segments = 10;

    private float angle;
    private float width;

    private float pitchChange = 0.2f;

    private ArcManager arcManager;
    public void Create(ArcManager arcManager, Vector3 direction, float width)
    {
        this.arcManager = arcManager;
        this.width = width;

        // create mesh. first vertex is the center of the circle, the rest are on the circle

        mesh = new Mesh();
        GameObject indicator = new GameObject("Indicator");
        indicator.transform.parent = this.transform;
        meshFilter = indicator.AddComponent<MeshFilter>();
        meshRenderer = indicator.AddComponent<MeshRenderer>();
        meshFilter.mesh = mesh;


        // caclulate angle from just direction
        angle = Mathf.Atan2(direction.normalized.z, direction.normalized.x) * Mathf.Rad2Deg;
        

        // ------------------------------------- MESH -----------------------------------------------------

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];


        vertices[0] = arcManager.transform.position; // circles center

        float radius = arcManager.circleRadius;
        float len = width; // angle of the arc
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
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        meshRenderer.material = materialWhite;

        


    }
    

    public IEnumerator FlashRoutine(float duration)
    {
        float elapsedTotalTime = 0f;
        float currentFlashDuration = duration / 10f; // Initial flash duration
        float minFlashDuration = 0.25f; // Minimum flash duration

        float startAlpha = materialWhite.color.a;

        while (elapsedTotalTime < duration)
        {
            // Ensure each flash is within the minimum duration
            currentFlashDuration = Mathf.Max(currentFlashDuration, minFlashDuration);

            // Fade in
            yield return StartCoroutine(FadeAlpha(0f, currentFlashDuration / 2));
            // Fade out
            yield return StartCoroutine(FadeAlpha(startAlpha, currentFlashDuration / 2));

            // Increase the elapsed total time
            elapsedTotalTime += currentFlashDuration;

            // Gradually decrease the flash duration to speed up
            currentFlashDuration *= 0.9f; // Adjust rate of speed increase here
        }

        Color finalColor = materialWhite.color;
        finalColor.a = startAlpha;
        materialWhite.color = finalColor;
    }

    private IEnumerator FadeAlpha(float targetAlpha, float duration)
    {
        float startAlpha = materialWhite.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            Color color = materialWhite.color;
            color.a = newAlpha;
            materialWhite.color = color;
            if (newAlpha == startAlpha && targetAlpha > startAlpha)
            {
                // change blip clips pitch a little higher through time to create a sense of urgency
                AudioManager.Instance.PlaySFX(blipClip).pitch += pitchChange;
                pitchChange += 0.2f;
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
        meshRenderer.material = materialAttack;
        // create lightning particle effect
    }
}
