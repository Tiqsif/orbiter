using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthBarFill;
    private Boss _boss;
    private void Awake()
    {
        _boss = FindObjectOfType<Boss>(); // there is always only one at the moment
    }

    private void Update()
    {
        _healthBarFill.fillAmount = _boss.currentHealth / _boss.maxHealth;
    }
}
