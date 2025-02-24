using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShot : MonoBehaviour
{
    [SerializeField] private float _jumpPower = 2f;
    [SerializeField] private AudioClip _hitClip;
    [SerializeField] private AudioClip _groundClip;

    private Vector3 _dir;
    private float _duration;

    public delegate void OnPlayerShotArrived(float damage);
    public static event OnPlayerShotArrived onPlayerShotArrived;

    public void Shoot(float damage, Vector3 destination, float duration)
    {
        // on complete call a function
        _dir = destination - transform.position;
        _duration = duration;
        transform.DOJump(destination, _jumpPower, 1, duration).onComplete += () => Complete(damage);
    }

    private void Complete(float damage)
    {
        onPlayerShotArrived?.Invoke(damage);
        AudioManager.Instance.KillSFX(_hitClip);
        AudioManager.Instance.PlaySFX(_hitClip);
        transform.DOJump(transform.position + _dir, _jumpPower * 4f, 1, _duration * 3f) // longer glide
            .onComplete += OnGround;
    }

    private void OnGround()
    {
        // find arcmanager by type
        AudioManager.Instance.KillSFX(_groundClip);
        AudioManager.Instance.PlaySFX(_groundClip);

        ArcManager arcManager = FindObjectOfType<ArcManager>();
        if (arcManager)
        {
            arcManager.AddPlayerShotOnGround(this);
        }


    }
}
