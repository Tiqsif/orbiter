using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int difficulty;
    public int attackCounter = 0;
    public float maxHealth;
    public float currentHealth;
    public Transform model;

    public AudioClip spawnClip;
    public AudioClip idleClip;
    public AudioClip deathClip;
    public AudioClip attackClip;
    protected ArcManager arcManager;
    public BossState currentState;
    [HideInInspector] public BossState previousState;

    public float spawnSpeed = 1.0f;

    protected Player player;
    public delegate void OnAttackCounterChange(int attackCounter);
    public static OnAttackCounterChange onAttackCounterChange;

    public delegate void OnBossAttack(int attackNo);
    public static OnBossAttack onBossAttack;

    public delegate void OnBossDie();
    public static OnBossDie onBossDie;
    public enum BossState
    {
        Spawn,
        Idle,
        Attack,
        Dead
    }

    protected void OnEnable()
    {
        Player.onPlayerShootBegin += OnPlayerShootBegin;
    }

    
    protected void OnDisable()
    {
        Player.onPlayerShootBegin -= OnPlayerShootBegin;
    }
    protected void Awake()
    {
        currentHealth = maxHealth;

    }
    protected void Start()
    {
        arcManager = FindObjectOfType<ArcManager>();
        player = FindObjectOfType<Player>();
        ChangeState(BossState.Spawn);
    }
    protected void Update()
    {
        
    }


    public virtual void Spawn()
    {
        //Debug.Log("Boss Spawn");

        AudioManager.Instance.KillSFX(spawnClip);
        AudioManager.Instance.PlaySFX(spawnClip, 0f, 0.75f);
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
            yield return null;
        }
        transform.position = targetPosition;
        ChangeState(BossState.Idle);
    }
    public virtual void Die()
    {
        //Debug.Log("Boss Die");
        StartCoroutine(DieRoutine());
    }

    public virtual IEnumerator DieRoutine()
    {
        yield return null;
        if (deathClip)
        {
            AudioManager.Instance.KillSFX(deathClip);
            AudioManager.Instance.PlaySFX(deathClip);
        }
        yield return null;
        onBossDie?.Invoke();
    }
    public virtual void Idle()
    {
        //Debug.Log("Boss Idle");
        StartCoroutine(IdleRoutine());
    }

    public virtual IEnumerator IdleRoutine()
    {
        yield return null;
        yield return new WaitForSeconds(2f);
        ChangeState(BossState.Attack);
    }

    public virtual void Attack()
    {
        //Debug.Log("Boss Attack");
        attackCounter ++;
        onAttackCounterChange?.Invoke(attackCounter);
        StartCoroutine(AttackRoutine());
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
        if(player.isDead) return;
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

    public virtual void OnPlayerShootBegin()
    {
        
        TakeDamage(10);
        
    }
}
