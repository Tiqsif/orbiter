using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAttack : MonoBehaviour
{
    public AudioClip blipClip;
    public Material materialWhite;
    public Material materialAttack;
    protected Vector3 direction; // direction of the orb, has the speed in it
    protected bool hasIndicator = false;
    protected AreaIndicator indicator;
    private ArcManager arcManager;

    private void Awake()
    {
        arcManager = FindObjectOfType<ArcManager>();
        materialWhite = new Material(materialWhite);
        materialAttack = new Material(materialAttack);

    }
    public void Activate(Vector3 direction, float arcAngle, float waitTime, float activeTime)
    {
        this.direction = direction;
        
        hasIndicator = true;

        GameObject indicatorObj = new GameObject("Indicator");
        indicator = indicatorObj.AddComponent<AreaIndicator>();
        indicator.materialWhite = materialWhite;
        indicator.materialAttack = materialAttack;
        indicator.blipClip = blipClip;
        indicator.Create(arcManager, direction, arcAngle);
        StartCoroutine(ActivateRoutine(arcAngle, waitTime, activeTime));

    }

    private IEnumerator ActivateRoutine(float arcAngle, float waitTime, float activeTime)
    {
        // create a visual indicator for the orb
        yield return StartCoroutine(indicator.FlashRoutine(waitTime));

        indicator.HitMode();
        float elepsedTime = 0;
        while (elepsedTime < activeTime)
        {
            arcManager.CheckAreaHit(direction, arcAngle);
            elepsedTime += Time.deltaTime;
            yield return null;
        }

        ClearIndicator();

    }

    

    public void ClearIndicator()
    {
        hasIndicator = false;
        // destroy the visual indicator maybe with an effect
        Destroy(indicator.gameObject);
    }
}
