using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalFXManager : MonoBehaviour
{
    [SerializeField] private Color[] _colors;
    [SerializeField] private GameObject _attackBeginParticlePrefab;
    [SerializeField] private AudioClip _attackBeginClip;
    private ParticleSystem _ps;
    private GameObject _currentAttackBeginParticle;
    private void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
        
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
        if (attackIndex < _colors.Length)
        {
            var main = _ps.main;
            main.startColor = _colors[attackIndex];
        }
        else
        {
            //Debug.LogWarning("attackIndex out of range: " + attackIndex);
            return;
        }
        if(_attackBeginParticlePrefab != null)
        {
            if (_currentAttackBeginParticle != null)
            {
                Destroy(_currentAttackBeginParticle);
            }
            _currentAttackBeginParticle = Instantiate(_attackBeginParticlePrefab, transform.position, Quaternion.identity, transform);
            _currentAttackBeginParticle.transform.localScale = Vector3.one * 3f;
            foreach (Transform child in _currentAttackBeginParticle.transform)
            {
                var ps = child.GetComponent<ParticleSystem>();
                var main = ps.main;
                  
                main.startColor = _colors[attackIndex];
                ps.Play();
            }

            if (_attackBeginClip != null)
            {
                AudioManager.Instance.KillSFX(_attackBeginClip);
                AudioManager.Instance.PlaySFX(_attackBeginClip, 0.6f);
            }
        }
    }
}
