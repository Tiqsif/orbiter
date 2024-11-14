using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAttack : MonoBehaviour
{
    public AudioClip blipClip;
    public AudioClip particleClip;
    public GameObject particlePrefab;
    public Material materialWhite;
    public Material materialAttack;
    protected Vector3 direction; // direction of the orb, has the speed in it
    protected bool hasIndicator = false;
    protected AreaIndicator indicator;
    private ArcManager arcManager;
    private GameObject particleHolder;
    private AudioSource source;
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
        ActivateParticles(arcAngle, activeTime);
        float elepsedTime = 0;
        while (elepsedTime < activeTime)
        {
            arcManager.CheckAreaHit(direction, arcAngle);
            elepsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(source.gameObject);
        ClearIndicator();
        ClearParticles();
    }

    public void ActivateParticles(float arcAngle, float activeTime)
    {
        float particleAngle = 30f;
        int numParticles = (int)(arcAngle / particleAngle);
        // instantiate particle prefab for every particleAngle in the arcAngle
        // starting from the direction - arcAngle / 2 to direction + arcAngle / 2
        for (int i = 0; i < numParticles; i++)
        {
            Vector3 particleDirection = Quaternion.Euler(0, (-arcAngle / 2) + i * particleAngle + particleAngle/2, 0) * direction;
            GameObject particle = Instantiate(particlePrefab, arcManager.transform.position, Quaternion.identity);
            if (particleHolder == null)
            {
                particleHolder = new GameObject("ParticleHolder");
                particleHolder.transform.parent = this.transform;
            }
            particle.transform.parent = particleHolder.transform;
            // apply the particledirection to the particle
            
            particle.transform.forward = particleDirection;
        }

        source = AudioManager.Instance.PlaySFX(particleClip, loop:true);
        source.pitch += Random.Range(0.3f,0.5f);
        source.loop = true;

    }

    public void ClearParticles()
    {
        Destroy(particleHolder);
    }

    public void ClearIndicator()
    {
        hasIndicator = false;
        // destroy the visual indicator maybe with an effect
        Destroy(indicator.gameObject);
    }
}
