using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] protected int _difficulty;
    [SerializeField] protected int _attackCounter = 0;
    public float maxHealth;
    public float currentHealth;
    [SerializeField] protected Transform _model;

    [SerializeField] protected AudioClip _spawnClip;
    [SerializeField] protected AudioClip _idleClip;
    [SerializeField] protected AudioClip _deathClip;
    [SerializeField] protected AudioClip _attackClip;
    protected ArcManager _arcManager;
    [SerializeField] protected BossState _currentState;
    protected BossState _previousState;

    [SerializeField] protected float _spawnDuration = 4.0f;

    protected Player _player;
    public delegate void OnAttackCounterChange(int attackCounter);
    public static OnAttackCounterChange onAttackCounterChange;

    public delegate void OnBossAttack(int attackNo);
    public static OnBossAttack onBossAttack;

    public delegate void OnBossDie();
    public static OnBossDie onBossDie;

    public delegate void OnBossTakeDamage(float damage);
    public static OnBossTakeDamage onBossTakeDamage;
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
        PlayerShot.onPlayerShotArrived += OnPlayerShotArrived;
    }

    
    protected void OnDisable()
    {
        Player.onPlayerShootBegin -= OnPlayerShootBegin;
        PlayerShot.onPlayerShotArrived -= OnPlayerShotArrived;
    }
    protected void Awake()
    {
        currentHealth = maxHealth;

    }
    protected void Start()
    {
        _arcManager = FindObjectOfType<ArcManager>();
        _player = FindObjectOfType<Player>();
        ChangeState(BossState.Spawn);
    }
    protected void Update()
    {
        
    }


    protected virtual void Spawn()
    {
        //Debug.Log("Boss Spawn");

        AudioManager.Instance.KillSFX(_spawnClip);
        AudioManager.Instance.PlaySFX(_spawnClip, 0f, 0.75f);
        StartCoroutine(SpawnRoutine());
        
    }
    protected virtual IEnumerator SpawnRoutine()
    {
        Vector3 targetPosition = transform.position;
        transform.position = new Vector3(targetPosition.x, targetPosition.y - 20, targetPosition.z);
        yield return null;
        transform.DOMoveY(targetPosition.y, _spawnDuration).SetEase(Ease.OutBack);
        /*
        Vector3 startPosition = new Vector3(targetPosition.x, targetPosition.y - 10, targetPosition.z);
        float spawnPercent = 0;
        while (spawnPercent < 1)
        {
            spawnPercent += Time.deltaTime * _spawnSpeed;
            float curvePercent = 1 - Mathf.Cos(spawnPercent * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(startPosition, targetPosition, curvePercent);
            yield return null;
        }
        */
        yield return new WaitForSeconds(_spawnDuration - 2f); // idle duration is 2 seconds
        yield return null;
        transform.position = targetPosition;
        ChangeState(BossState.Idle);
    }
    protected virtual void Die()
    {
        //Debug.Log("Boss Die");
        StartCoroutine(DieRoutine());
    }

    protected virtual IEnumerator DieRoutine()
    {
        yield return null;
        if (_deathClip)
        {
            AudioManager.Instance.KillSFX(_deathClip);
            AudioManager.Instance.PlaySFX(_deathClip);
        }
        yield return null;
        onBossDie?.Invoke();
    }
    protected virtual void Idle()
    {
        //Debug.Log("Boss Idle");
        StartCoroutine(IdleRoutine());
    }

    protected virtual IEnumerator IdleRoutine()
    {
        yield return null;
        yield return new WaitForSeconds(2f);
        ChangeState(BossState.Attack);
    }

    protected virtual void Attack()
    {
        //Debug.Log("Boss Attack");
        _attackCounter ++;
        onAttackCounterChange?.Invoke(_attackCounter);
        StartCoroutine(AttackRoutine());
    }

    protected virtual IEnumerator AttackRoutine()
    {
        yield return null;
    }

    protected virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        onBossTakeDamage?.Invoke(damage);
        if (currentHealth <= 0)
        {
            ChangeState(BossState.Dead);
        }
    }

    protected virtual void ChangeState(BossState newState)
    {
        if (_player.isDead)
        {
            //Debug.Log("Boss:PlayerisdeadReturn");
            return;
        }
        _previousState = _currentState;
        _currentState = newState;
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

    protected virtual void OnPlayerShootBegin(float damage)
    {
        
        //TakeDamage(damage);
        
    }

    protected virtual void OnPlayerShotArrived(float damage)
    {
        if (_currentState == BossState.Dead)
        {
            return;
        }
        TakeDamage(damage);
    }
   
}
