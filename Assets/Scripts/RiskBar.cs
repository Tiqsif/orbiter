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
    public Image frame;
    public ParticleSystem fullParticle;
    public ParticleSystem shootParticle;

    bool isFull = false;
    Color frameStartColor;
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

    private void Awake()
    {
        frameStartColor = frame.color;
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
        shootParticle.Play();
        SetFillAmount();
    }

    void SetFillAmount()
    {
        riskBar.fillAmount = currentRisk / maxRisk;
        if (isFull)
        {
            fullParticle.Play();
            //frame.color = Color.white;
        }
        else
        {
            fullParticle.Stop();
            //frame.color = frameStartColor;
        }
    }
}
