using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalFXManager : MonoBehaviour
{
    public Color[] colors;
    private ParticleSystem ps;
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
    }
}
