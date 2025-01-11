using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public AudioClip changeDirectionClip;
    public AudioClip shootClip;

    public void PlayChangeDirection()
    {
        AudioManager.Instance.KillSFX(changeDirectionClip);
        AudioManager.Instance.PlaySFX(changeDirectionClip);
    }

    public void PlayShoot()
    {
        AudioManager.Instance.KillSFX(shootClip);
        AudioManager.Instance.PlaySFX(shootClip);
    }
}
