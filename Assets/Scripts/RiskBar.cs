using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RiskBar : MonoBehaviour
{
    [SerializeField] private float _maxRisk = 100f;
    [SerializeField] private float _currentRisk = 0f;
    [SerializeField] private float _riskIncrease = 2.5f;

    [SerializeField] private Image _riskBarFill;
    [SerializeField] private Image _barFrame;
    [SerializeField] private ParticleSystem _fullParticle;
    [SerializeField] private ParticleSystem _shootParticle;

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
        frameStartColor = _barFrame.color;
    }

    private void Start()
    {
        SetFillAmount();
    }

    private void OnRiskZoneHit()
    {
        _currentRisk += _riskIncrease; // this method is called in fixed update somewhere
        if (_currentRisk >= _maxRisk)
        {
            _currentRisk = _maxRisk;
            isFull = true;
            onRiskBarFull?.Invoke();
        }
        SetFillAmount();
    }

    public void ResetRisk(float _)
    {
        _currentRisk = 0;
        isFull = false;
        _shootParticle.Play();
        SetFillAmount();
    }

    void SetFillAmount()
    {
        _riskBarFill.fillAmount = _currentRisk / _maxRisk;
        if (isFull)
        {
            _fullParticle.Play();
            //frame.color = Color.white;
        }
        else
        {
            _fullParticle.Stop();
            //frame.color = frameStartColor;
        }
    }
}
