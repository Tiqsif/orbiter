using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMenu : MonoBehaviour
{
    [SerializeField] private RectTransform _panel;
    [SerializeField] private RectTransform _button;
    private bool _isOn = false;
    private float _waitTime = 0.5f;

    private void Start()
    {
        Time.timeScale = 1f;
    }
    private void OnEnable()
    {
        ArcManager.onPlayerHit += OnPlayerHit;
    }

    private void OnDisable()
    {
        ArcManager.onPlayerHit -= OnPlayerHit;
    }

    private void OnPlayerHit()
    {
        _button.gameObject.SetActive(false);
        _panel.gameObject.SetActive(true);
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
        // Restart the scene
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
