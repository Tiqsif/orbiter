using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalFXManager : MonoBehaviour
{
    public Color[] colors;
    public GameObject attackBeginParticlePrefab;
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
        Debug.Log("OnBossAttack: " + attackIndex);
        if (attackIndex < colors.Length)
        {
            var main = ps.main;
            main.startColor = colors[attackIndex];
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
        }
    }
}
