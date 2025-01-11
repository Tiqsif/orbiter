using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPS : MonoBehaviour
{
    public TextMeshProUGUI text;
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        
    }

    // Update is called once per frame
    void Update()
    {
        // update fps text on every second

        if (Time.frameCount % 60 == 0)
        {
            text.text = "FPS: " + (1 / Time.unscaledDeltaTime).ToString("F0");
        }

    }
}
