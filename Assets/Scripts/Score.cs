using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _multiplierText;
    [SerializeField] private ParticleSystem _multiplierGlowParticle;
    private int _score = 0;
    private int _scoreMultiplier = 1;
    private Tween _multiplierShakeLoop;
    private void Awake()
    {
        _scoreText.text = _score.ToString();
        _multiplierText.text = "x" + _scoreMultiplier.ToString();
        _multiplierText.gameObject.SetActive(false); // will set true after x2

        _multiplierShakeLoop = _multiplierText.rectTransform.DOShakeAnchorPos(1f, new Vector3(10, 10, 0), 10, 90f, false).SetLoops(-1, LoopType.Restart).SetAutoKill(false).Pause();
    }
    private void OnEnable()
    {
        Boss.onAttackCounterChange += UpdateScore;
        PlayerShot.onPlayerShotArrived += IncreaseScoreMultiplier;
    }

    private void OnDisable()
    {
        Boss.onAttackCounterChange -= UpdateScore;
        PlayerShot.onPlayerShotArrived -= IncreaseScoreMultiplier;
    }

    private void UpdateScore(int attackCounter)
    {
        _score = attackCounter * _scoreMultiplier;
        _scoreText.text = _score.ToString();
    }

    private void IncreaseScoreMultiplier(float _)
    {
        _scoreMultiplier++;
        HandleScoreModifierTM();
        _multiplierGlowParticle?.Play();
    }

    private void HandleScoreModifierTM()
    {
        // set multiplier text to active, set text, and tween
        _multiplierShakeLoop.Pause();
        float duration = 0.5f;
        _multiplierText.gameObject.SetActive(true);
        _multiplierText.text = "x" + _scoreMultiplier.ToString();
        float originalScale = _multiplierText.transform.localScale.x;
        // scale tween
        _multiplierText.rectTransform.DOScale(originalScale * 3f, duration).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            _multiplierText.rectTransform.DOScale(originalScale, duration).SetEase(Ease.OutBounce);
        });

        // move to down right tween
        Vector3 originalPos = _multiplierText.rectTransform.anchoredPosition;
        Vector3 targetPos = new Vector3(originalPos.x + 100, originalPos.y - 100, originalPos.z);

        
        _multiplierText.rectTransform.DOAnchorPos(targetPos, duration).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            _multiplierText.rectTransform.DOAnchorPos(originalPos, duration).SetEase(Ease.OutBounce).onComplete += () =>
            {
                _multiplierShakeLoop.Restart(); // after duration * 2
            };
        });

        // punch rotate tween
        _multiplierText.rectTransform.DOPunchRotation(new Vector3(0, 0, 10), duration, 10, 1f);

    }
}
