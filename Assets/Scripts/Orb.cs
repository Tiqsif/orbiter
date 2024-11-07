using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    public float width = 1;
    public Material indicatorMaterial0;
    public Material indicatorMaterial1;
    protected Vector3 direction; // direction of the orb, has the speed in it
    protected bool hasIndicator = false;
    protected AttackIndicator indicator;
    private ArcManager arcManager;
    public delegate void OrbArrived(Orb orb);
    public static event OrbArrived OnOrbArrived;
    private void Awake()
    {
        arcManager = FindObjectOfType<ArcManager>();
        
    }
    private void Start()
    {
        width = GetComponentInChildren<MeshFilter>().mesh.bounds.size.x; // or size.z depending on orientation
    }

    public void CreateIndicator()
    {
        // create a visual indicator for the orb
        hasIndicator = true;
        // create a mesh from center to the ending point on the circle with low alpha color
        GameObject indicatorObj = new GameObject("Indicator");
        indicator = indicatorObj.AddComponent<AttackIndicator>();
        indicator.material0 = indicatorMaterial0;
        indicator.material1 = indicatorMaterial1;
        indicator.Create(arcManager, direction, width);
        // create a mesh from center to the orb position with high alpha color
    }


    protected void ClearIndicator()
    {
        hasIndicator = false;
        // destroy the visual indicator maybe with an effect
    }

    public void Launch(Vector3 launchDirection)
    {
        float distance = arcManager.circleRadius;
        StartCoroutine(LaunchRoutine(launchDirection, distance));
    }
    IEnumerator LaunchRoutine(Vector3 launchDirection, float distance)
    {
        // direction has the speed in it
        direction = launchDirection;
        float distanceTravelled = 0;
        while (distanceTravelled < distance)
        {
            // move in the direction of the launch direction by the distance
            transform.position += direction * Time.deltaTime;
            distanceTravelled += direction.magnitude * Time.deltaTime;

            if (hasIndicator)
            {
                indicator.UpdateMesh(distanceTravelled/distance);
            }
            yield return null;
        }
        transform.position = direction.normalized * distance;
        indicator.UpdateMesh(1f);
        ClearIndicator();
        OnOrbArrived?.Invoke(this);
    }
}
