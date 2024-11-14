using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public AudioClip changeDirection;

    public void PlayChangeDirection()
    {
        AudioManager.Instance.KillSFX(changeDirection);
        AudioManager.Instance.PlaySFX(changeDirection);
    }
}
