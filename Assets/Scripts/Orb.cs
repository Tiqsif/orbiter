using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    public float width = 1;
    protected Vector3 direction; // direction of the orb, has the speed in it

    public delegate void OrbArrived(Orb orb);
    public static event OrbArrived OnOrbArrived;
    private void Awake()
    {
        
    }
    public void Launch(Vector3 launchDirection, float distance)
    {
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
            yield return null;
        }
        transform.position = direction.normalized * distance;
        OnOrbArrived?.Invoke(this);
    }
}
