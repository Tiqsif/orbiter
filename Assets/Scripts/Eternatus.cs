using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eternatus : Boss
{
    //private int totalAttacks = 2;
    public Transform baseEmitter;
    public Transform mouthEmitter;
    public GameObject projectilePrefab;
    public float projectileSpeed = 5.0f;

    private Animator animator; // temp, create a separate script for this!

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public override IEnumerator AttackRoutine()
    {
        yield return base.AttackRoutine();
        float offset = Random.Range(0, 360);
        yield return StartCoroutine(SymmetricalProjectile(3, offset, 1.5f));
        /*
        int attackNo = Random.Range(0, totalAttacks);
        switch (attackNo)
        {
            case 0:
                StartCoroutine(SymmetricalProjectile(3, 0f));
                break;
            case 1:
                StartCoroutine(SymmetricalProjectile(5, 0f));
                break;
        }
        */
        ChangeState(BossState.Idle);
    }

    public IEnumerator SymmetricalProjectile(int n, float angleOffset, float delay)
    {
        animator.SetTrigger("ProjectileAttack");

        // create an energy field with shaders maybe then spawn n projectiles symetrically.
        // 360/ n = angle between each projectile, angleOffset = offset from the baseEmitter's forward direction

        float angleStep = 360f / n; // angle between each projectile

        for (int i = 0; i < n; i++)
        {
            yield return new WaitForSeconds(delay);
            float currentAngle = i * angleStep + angleOffset; // angle of the current projectile
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * baseEmitter.forward; // direction of the current projectile

            GameObject projectile = Instantiate(projectilePrefab, baseEmitter.position, Quaternion.identity);
            if (projectile.TryGetComponent(out Orb orb))
            {
                orb.Launch(direction * projectileSpeed);
                orb.CreateIndicator();
            }
            //Debug.Log(direction.normalized * projectileSpeed);

            Debug.DrawLine(baseEmitter.position, baseEmitter.position + direction.normalized * 2, Color.red, 5f);

        }

        yield return new WaitForSeconds(delay);
    }
}
