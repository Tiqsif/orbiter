using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSFX : MonoBehaviour
{
    public float stepVolume = 0.05f;
    public AudioClip[] stepClips;

    public void PlayStepSFX()
    {
        /*
        if (Random.Range(0, 100) > 50)
        {
            return;
        }
        */
        for(int i = 0; i < stepClips.Length; i++)
        {

            AudioManager.Instance.KillSFX(stepClips[i]);
        }
        
        AudioClip stepClip = stepClips[Random.Range(0, stepClips.Length)];
        AudioManager.Instance.PlaySFX(stepClip, Random.Range(0f,0.025f), stepVolume);
    }
}
