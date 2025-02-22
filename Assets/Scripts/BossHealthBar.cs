using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthBarFill;
    [SerializeField] private ParticleSystem _hitParticle;
    private Boss _boss;
    private void Awake()
    {
        _boss = FindObjectOfType<Boss>(); // there is always only one at the moment

    }

    private void Start()
    {
        _healthBarFill.fillAmount = _boss.currentHealth / _boss.maxHealth;
    }
    private void OnEnable()
    {
        Boss.onBossTakeDamage += OnBossTakeDamage;
    }

    private void OnDisable()
    {
        Boss.onBossTakeDamage -= OnBossTakeDamage;
    }

    

    private void OnBossTakeDamage(float damage)
    {
        _healthBarFill.fillAmount = _boss.currentHealth / _boss.maxHealth;
        transform.DOPunchRotation(new Vector3(0, 0, 15), 0.75f, 10, 0.2f);
        _hitParticle.Play();
    }
}
