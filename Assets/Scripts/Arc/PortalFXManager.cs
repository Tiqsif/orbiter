using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalFXManager : MonoBehaviour
{
    public Color[] colors;
    public GameObject attackBeginParticlePrefab;
    public AudioClip attackBeginClip;
    private ParticleSystem ps;
    private GameObject currentAttackBeginParticle;
    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        
    }
    private void OnEnable()
    {
        Boss.onBossAttack += OnBossAttack;
    }

    private void OnDisable()
    {
        Boss.onBossAttack -= OnBossAttack;
    }

    private void OnBossAttack(int attackIndex)
    {
        //Debug.Log("OnBossAttack: " + attackIndex);
        if (attackIndex < colors.Length)
        {
            var main = ps.main;
            main.startColor = colors[attackIndex];
        }
        else
        {
            //Debug.LogWarning("attackIndex out of range: " + attackIndex);
            return;
        }
        if(attackBeginParticlePrefab != null)
        {
            if (currentAttackBeginParticle != null)
            {
                Destroy(currentAttackBeginParticle);
            }
            currentAttackBeginParticle = Instantiate(attackBeginParticlePrefab, transform.position, Quaternion.identity, transform);
            currentAttackBeginParticle.transform.localScale = Vector3.one * 3f;
            foreach (Transform child in currentAttackBeginParticle.transform)
            {
                var ps = child.GetComponent<ParticleSystem>();
                var main = ps.main;
                  
                main.startColor = colors[attackIndex];
                ps.Play();
            }

            if (attackBeginClip != null)
            {
                AudioManager.Instance.KillSFX(attackBeginClip);
                AudioManager.Instance.PlaySFX(attackBeginClip, 0.6f);
            }
        }
    }
}
