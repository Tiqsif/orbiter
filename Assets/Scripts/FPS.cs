using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPS : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fpsText; // assigned in inspector
    [SerializeField] private TextMeshProUGUI _msText; // assigned in inspector
    

    // Update is called once per frame
    void Update()
    {
        // update fps text on every second
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;

        if (Time.frameCount % 10 == 0)
        {
            _fpsText.text = "FPS: " + (1 / Time.unscaledDeltaTime).ToString("F0");
            _msText.text = "ms: " + (Time.unscaledDeltaTime * 1000f).ToString("F2");
        }

    }
}
