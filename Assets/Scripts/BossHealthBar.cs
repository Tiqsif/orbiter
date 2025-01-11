using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Image healthBar;
    private Boss boss;
    private void Awake()
    {
        boss = FindObjectOfType<Boss>(); // there is always only one at the moment
    }

    private void Update()
    {
        healthBar.fillAmount = boss.currentHealth / boss.maxHealth;
    }
}
