using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WinMenu : MonoBehaviour
{
    public RectTransform panel;
    private bool isOn = false;
    private float waitTime = 1f;

    private void Start()
    {
        Time.timeScale = 1f;
    }
    private void OnEnable()
    {
        Boss.onBossDie += OnBossDie;
    }

    private void OnDisable()
    {
        Boss.onBossDie -= OnBossDie;
    }

    private void OnBossDie()
    {
        panel.gameObject.SetActive(true);
        isOn = true;
        Time.timeScale = 0.01f;

    }

    private void Update()
    {
        if (!isOn) return;
        waitTime -= Time.deltaTime / Time.timeScale;
        waitTime = Mathf.Max(waitTime, 0);
        if (isOn && waitTime <= 0 && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                panel.gameObject.SetActive(false);
                isOn = false;
                // Restart the scene
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

            }
        }
    }
}

