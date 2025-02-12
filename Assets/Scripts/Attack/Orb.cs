using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    public float width = 1;
    public Material indicatorMaterial0;
    public Material indicatorMaterial1;
    protected Vector3 _direction; // direction of the orb, has the speed in it
    protected bool _hasIndicator = false;
    protected ProjectileIndicator _indicator;
    private ArcManager _arcManager;

    public delegate void OrbArrived(Orb orb);
    public static event OrbArrived OnOrbArrived;
    private void Awake()
    {
        _arcManager = FindObjectOfType<ArcManager>();
        width = GetComponentInChildren<MeshFilter>().gameObject.transform.localScale.x;
        //Debug.Log("Orb width: " + width);
        
    }
    private void Start()
    {
    }

    public void CreateIndicator()
    {
        // create a visual indicator for the orb
        _hasIndicator = true;
        // create a mesh from center to the ending point on the circle with low alpha color
        GameObject indicatorObj = new GameObject("Indicator");
        _indicator = indicatorObj.AddComponent<ProjectileIndicator>();
        _indicator.material0 = indicatorMaterial0;
        _indicator.material1 = indicatorMaterial1;
        _indicator.Create(_arcManager, _direction, width);
        // create a mesh from center to the orb position with high alpha color
    }


    protected void ClearIndicator()
    {
        _hasIndicator = false;
        // destroy the visual indicator maybe with an effect
        Destroy(_indicator.gameObject);
    }

    public void Launch(Vector3 launchDirection)
    {
        float distance = _arcManager.circleRadius;
        StartCoroutine(LaunchRoutine(launchDirection, distance));
        float time = distance / launchDirection.magnitude;
        _arcManager.CreateRiskZone(launchDirection, Mathf.Rad2Deg * width / _arcManager.circleRadius, time);
    }
    IEnumerator LaunchRoutine(Vector3 launchDirection, float distance)
    {
        // direction has the speed in it
        _direction = launchDirection;
        float distanceTravelled = 0;
        while (distanceTravelled < distance)
        {
            // move in the direction of the launch direction by the distance
            transform.position += _direction * Time.deltaTime;
            distanceTravelled += _direction.magnitude * Time.deltaTime;

            if (_hasIndicator)
            {
                _indicator.UpdateMesh(distanceTravelled/distance);
            }
            yield return null;
        }
        transform.position = _direction.normalized * distance;
        _indicator.UpdateMesh(1f);
        ClearIndicator();
        OnOrbArrived?.Invoke(this);
        Destroy(gameObject);
    }
}
