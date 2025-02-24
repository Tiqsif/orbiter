using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RiskBar : MonoBehaviour
{
    [SerializeField] private float _maxRisk = 100f;
    [SerializeField] private float _currentRisk = 0f;
    [SerializeField] private float _riskIncrease = 2.5f;

    [SerializeField] private Image _riskBarFill;
    [SerializeField] private Image _barFrame;
    [SerializeField] private ParticleSystem _fullParticle;
    [SerializeField] private ParticleSystem _shootParticle;
    [SerializeField] private ParticleSystem _increaseParticle;
    [SerializeField] private AudioClip _increaseClip;
    [SerializeField] private AudioClip _increaseByPickupClip;

    bool isFull = false;
    Color frameStartColor;

    private Tween _increaseTween;
    private Tween _increaseByPickupTween;
    private float _scaleDuration = 0.15f;
    private Sequence _fullSequence;

    public delegate void OnRiskBarFull();
    public static OnRiskBarFull onRiskBarFull;

    private void OnEnable()
    {
        ArcManager.onRiskZoneHit += OnRiskZoneHit;
        ArcManager.onPlayerShotPickup += PlayerShotPickUp;
        Player.onPlayerShootBegin += PlayerShoot;
    }

    private void OnDisable()
    {
        ArcManager.onRiskZoneHit -= OnRiskZoneHit;
        ArcManager.onPlayerShotPickup -= PlayerShotPickUp;
        Player.onPlayerShootBegin -= PlayerShoot;
    }

    private void Awake()
    {
        frameStartColor = _barFrame.color;

        _increaseTween = transform.DOScale(transform.localScale * 1.1f, _scaleDuration/2f)
           .SetEase(Ease.OutQuad)
           .SetLoops(2, LoopType.Yoyo) // expand and contract
           .SetAutoKill(false)
           .Pause();

        _increaseByPickupTween = transform.DOScale(transform.localScale * 1.8f, _scaleDuration * 2)
            .SetEase(Ease.OutQuad)
            .SetLoops(2, LoopType.Yoyo) // expand and contract
            .SetAutoKill(false)
            .Pause();

        /*
        _shakeTween = transform.DOShakeRotation(0.1f, new Vector3(0, 0, 2), 10, 90f, false, ShakeRandomnessMode.Full)
            .SetAutoKill(false)
            .SetLoops(-1, LoopType.Restart)
            .Pause();
        */
        _fullSequence = DOTween.Sequence()
            .Append(transform.DOMoveX(-1, 0.2f).SetEase(Ease.InOutSine))
            .Append(transform.DOMoveX(1, 0.2f).SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Yoyo)
            .SetAutoKill(false)
            .Pause();
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
        Invoke(nameof(BarIncreaseFX), 0.0f);
    }

    private void PlayerShotPickUp()
    {
        _currentRisk += _maxRisk/2f; // half the bar
        if (_currentRisk >= _maxRisk)
        {
            _currentRisk = _maxRisk;
            isFull = true;
            onRiskBarFull?.Invoke();
        }
        SetFillAmount();
        Invoke(nameof(BarIncreaseByPickupFX), 0.0f);
    }

    public void PlayerShoot(float _)
    {
        float punchStrength = 25f;
        float duration = 0.75f;
        transform.DOShakePosition(duration, new Vector3(punchStrength, punchStrength, 0f), 10, 90f, false, true, ShakeRandomnessMode.Harmonic);
        transform.DOPunchScale(Vector3.one * 0.8f, duration);

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
            if (!_fullSequence.IsPlaying()) _fullSequence.Restart();
            //frame.color = Color.white;
        }
        else
        {
            _fullParticle.Stop();
            _fullSequence.Restart();
            _fullSequence.Pause();
            //frame.color = frameStartColor;
        }
    }

    private void BarIncreaseFX()
    {
        if (_increaseTween.IsPlaying() || isFull) return;

        _increaseTween.Restart();
        AudioManager.Instance.KillSFX(_increaseClip);
        AudioManager.Instance.PlaySFX(_increaseClip);
        _increaseParticle.Play();
    }

    private void BarIncreaseByPickupFX()
    {
        if (!_increaseByPickupTween.IsPlaying())
        {
            _increaseByPickupTween.Restart();
        }

        AudioManager.Instance.KillSFX(_increaseByPickupClip);
        AudioManager.Instance.PlaySFX(_increaseByPickupClip);
        _increaseParticle.Play();
    }
}
