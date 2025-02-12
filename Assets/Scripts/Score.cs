using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    private TextMeshProUGUI _scoreText;


    private void Awake()
    {
        _scoreText = GetComponent<TextMeshProUGUI>();
        _scoreText.text = "0";
    }
    private void OnEnable()
    {
        Boss.onAttackCounterChange += UpdateScore;
    }

    private void OnDisable()
    {
        Boss.onAttackCounterChange -= UpdateScore;
    }

    private void UpdateScore(int attackCounter)
    {
        _scoreText.text = attackCounter.ToString();
    }
}
