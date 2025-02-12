using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSFX : MonoBehaviour
{
    [SerializeField] private float _stepVolumeModifier = 1f;
    [SerializeField] private AudioClip[] _stepClips;

    public void PlayStepSFX()
    {
        /*
        if (Random.Range(0, 100) > 50)
        {
            return;
        }
        */
        for(int i = 0; i < _stepClips.Length; i++)
        {

            AudioManager.Instance.KillSFX(_stepClips[i]);
        }
        
        AudioClip stepClip = _stepClips[Random.Range(0, _stepClips.Length)];
        AudioManager.Instance.PlaySFX(stepClip, Random.Range(0f,0.025f), _stepVolumeModifier);
    }
}
