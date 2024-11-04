using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public BossState currentState;
    [HideInInspector] public BossState previousState;

    public float spawnSpeed = 1.0f;
    public enum BossState
    {
        Spawn,
        Idle,
        Attack,
        Dead
    }

    private void Awake()
    {
        currentHealth = maxHealth;

    }
    private void Start()
    {
        ChangeState(BossState.Spawn);
    }


    public virtual void Spawn()
    {
        Debug.Log("Boss Spawn");
        StartCoroutine(SpawnRoutine());
        
    }
    public virtual IEnumerator SpawnRoutine()
    {
        Vector3 targetPosition = transform.position;
        Vector3 startPosition = new Vector3(targetPosition.x, targetPosition.y - 10, targetPosition.z);
        float spawnPercent = 0;
        while (spawnPercent < 1)
        {
            spawnPercent += Time.deltaTime * spawnSpeed;
            float curvePercent = 1 - Mathf.Cos(spawnPercent * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(startPosition, targetPosition, curvePercent);
            Debug.Log("Spawn Routine");
            yield return null;
        }
        transform.position = targetPosition;
        ChangeState(BossState.Idle);
    }
    public virtual void Die()
    {
        Debug.Log("Boss Die");
    }

    public virtual IEnumerator DieRoutine()
    {
        yield return null;
    }
    public virtual void Idle()
    {
        Debug.Log("Boss Idle");
    }

    public virtual IEnumerator IdleRoutine()
    {
        yield return null;
    }

    public virtual void Attack()
    {
        Debug.Log("Boss Attack");
    }

    public virtual IEnumerator AttackRoutine()
    {
        yield return null;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            ChangeState(BossState.Dead);
        }
    }

    public virtual void ChangeState(BossState newState)
    {
        previousState = currentState;
        currentState = newState;
        switch (newState)
        {
            case BossState.Spawn:
                Spawn();
                break;
            case BossState.Idle:
                Idle();
                break;
            case BossState.Attack:
                Attack();
                break;
            case BossState.Dead:
                Die();
                break;
        }
    }
}
