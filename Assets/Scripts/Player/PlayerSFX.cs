using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [SerializeField] private AudioClip _changeDirectionClip;
    [SerializeField] private AudioClip _shootClip;

    public void PlayChangeDirection()
    {
        AudioManager.Instance.KillSFX(_changeDirectionClip);
        AudioManager.Instance.PlaySFX(_changeDirectionClip);
    }

    public void PlayShoot()
    {
        AudioManager.Instance.KillSFX(_shootClip);
        AudioManager.Instance.PlaySFX(_shootClip);
    }
}
