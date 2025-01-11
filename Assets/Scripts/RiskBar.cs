using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RiskBar : MonoBehaviour
{
    public float maxRisk = 500f;
    public float currentRisk = 0f;
    public float riskIncrease = 1f;

    public Image riskBar;

    bool isFull = false;

    public delegate void OnRiskBarFull();
    public static OnRiskBarFull onRiskBarFull;

    private void OnEnable()
    {
        ArcManager.onRiskZoneHit += OnRiskZoneHit;
        Player.onPlayerShootBegin += ResetRisk;
    }

    private void OnDisable()
    {
        ArcManager.onRiskZoneHit -= OnRiskZoneHit;
        Player.onPlayerShootBegin -= ResetRisk;
    }

    private void Start()
    {
        SetFillAmount();
    }

    private void OnRiskZoneHit()
    {
        currentRisk += riskIncrease; // this method is called in fixed update somewhere
        if (currentRisk >= maxRisk)
        {
            currentRisk = maxRisk;
            isFull = true;
            onRiskBarFull?.Invoke();
        }
        SetFillAmount();
    }

    public void ResetRisk()
    {
        currentRisk = 0;
        isFull = false;
        SetFillAmount();
    }

    void SetFillAmount()
    {
        riskBar.fillAmount = currentRisk / maxRisk;

    }
}
