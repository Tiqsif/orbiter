using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPS : MonoBehaviour
{
    public TextMeshProUGUI fpsText; // assigned in inspector
    public TextMeshProUGUI msText; // assigned in inspector
    

    // Update is called once per frame
    void Update()
    {
        // update fps text on every second
        Application.targetFrameRate = 30;//(int)Screen.currentResolution.refreshRateRatio.value;

        if (Time.frameCount % 10 == 0)
        {
            fpsText.text = "FPS: " + (1 / Time.unscaledDeltaTime).ToString("F0");
            msText.text = "ms: " + (Time.unscaledDeltaTime * 1000f).ToString();
        }

    }
}
