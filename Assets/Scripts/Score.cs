using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI scoreText;


    private void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        scoreText.text = "0";
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
        scoreText.text = attackCounter.ToString();
    }
}
