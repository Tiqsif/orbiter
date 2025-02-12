using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WinMenu : MonoBehaviour
{
    [SerializeField] private RectTransform _panel;
    [SerializeField] private RectTransform _button;
    private bool _isOn = false;
    private float _waitTime = 1f;

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
        _panel.gameObject.SetActive(true);
        _button.gameObject.SetActive(false);
        _isOn = true;
        Invoke("SetInputAvailable", _waitTime);

    }

    private void SetInputAvailable()
    {
        if (_isOn)
        {
            _button.gameObject.SetActive(true);
        }
        
    }


    public void OnRestartButtonClicked()
    {
        _button.gameObject.SetActive(false);
        _panel.gameObject.SetActive(false);
        _isOn = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void Update()
    {
        /*
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
        */
    }
}

