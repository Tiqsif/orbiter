using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMenu : MonoBehaviour
{
    public RectTransform panel;
    public RectTransform button;
    private bool isOn = false;
    private float waitTime = 1f;

    private void Start()
    {
        Time.timeScale = 1f;
        if (isOn)
        {

        }
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
        button.gameObject.SetActive(false);
        panel.gameObject.SetActive(true);
        isOn = true;
        Invoke("SetInputAvailable", waitTime);

    }

    private void SetInputAvailable()
    {
        button.gameObject.SetActive(true);
        
    }



    public void OnRestartButtonClicked()
    {
        button.gameObject.SetActive(false);
        panel.gameObject.SetActive(false);
        isOn = false;
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
