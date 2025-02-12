using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eternatus : Boss
{
    //private int totalAttacks = 2;
    [SerializeField] private Transform _baseEmitter;
    [SerializeField] private Transform _mouthEmitter;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _areaAttackPrefab;
    [SerializeField] private float _projectileSpeed = 12.0f;
    [SerializeField] private AudioClip _roarClip;
    private Animator _animator; // temp, create a separate script for this!
    private int _maxDifficulty = 20;
    protected new void Awake()
    {
        base.Awake();
        _animator = GetComponentInChildren<Animator>();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (_player && _arcManager) Gizmos.DrawLine(_baseEmitter.position, _arcManager.GetPositionFromAngle(_arcManager.GetAngleFromPosition(_player.GetFollowerPosition())));
    }
    protected override IEnumerator AttackRoutine()
    {
        yield return base.AttackRoutine();

        int attackNo = Random.Range(0, 4);
        onBossAttack?.Invoke(attackNo);
        AudioManager.Instance.KillSFX(_roarClip);
        AudioManager.Instance.PlaySFX(_roarClip, 0f, 0.6f);
        switch (attackNo)
        {
            case 0:
                yield return StartCoroutine(SymmetricalProjectile(Mathf.Min(_attackCounter, _maxDifficulty))); // order is broken
                break;
            case 1:
                yield return StartCoroutine(SymmetricalProjectileInstant(Mathf.Min(_attackCounter, _maxDifficulty))); // shoots one doesnt rotate to the angle
                break;
            case 2:
                yield return StartCoroutine(AreaAttack(Mathf.Min(_attackCounter, _maxDifficulty)));
                break;
            case 3:
                yield return StartCoroutine(ProjectileInstant(Mathf.Min(_attackCounter, _maxDifficulty)));
                break;
        }
        ChangeState(BossState.Idle);
    }

    // ---------------------------------- Attacks ----------------------------------


    // ---------------------------------- Symmetrical Projectile ----------------------------------
    public IEnumerator SymmetricalProjectile(int n, float angleOffset, float delay)
    {
        // Calculate the angle step based on the number of projectiles
        float angleStep = 360f / n; // Angle between each projectile

        for (int i = 0; i < n; i++)
        {
            // Trigger the attack animation and play the sound
            _animator?.ResetTrigger("Attack");
            _animator?.SetTrigger("Attack");
            AudioManager.Instance.KillSFX(_attackClip);
            AudioManager.Instance.PlaySFX(_attackClip, 0f, 0.6f);

            // Calculate the current projectiles angle and direction
            float currentAngle = i * angleStep + angleOffset; // Angle of the current projectile
            currentAngle = (currentAngle + 360) % 360; // Normalize the angle to [0, 360)

            // always get the orb direction from the arcManager GetPositionFromAngle
            Vector3 direction = _arcManager.GetPositionFromAngle(currentAngle).normalized;
            //Debug.Log("direction: " + Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg);
            //Debug.Log("currentAngle: " + currentAngle);

            Quaternion targetRotation = Quaternion.LookRotation(direction); // Target rotation for the model
            //Debug.Log("targetRotation: " + Mathf.Atan2(targetRotation.eulerAngles.z, targetRotation.eulerAngles.x) * Mathf.Rad2Deg);
            float elapsedTime = 0f;
            float angularDistance = Quaternion.Angle(_model.rotation, targetRotation); // Angular distance between models current and target rotation
            float rotationSpeed = 5 * angularDistance / delay; // v = x/t, 5 is arbitrary
            rotationSpeed = Mathf.Lerp(rotationSpeed, rotationSpeed * 5, _difficulty/_maxDifficulty); // increase rotation speed with difficulty
            // wait before firing the projectile and smoothly rotate the model towards the target angle
            while (elapsedTime < delay)
            {
                elapsedTime += Time.deltaTime;
                // Smoothly rotate the model towards the target angle
                _model.rotation = Quaternion.RotateTowards(_model.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                yield return null;
            }

            // Instantiate the projectile and launch it in the calculated direction
            //Quaternion quaternion = Quaternion.Euler(0, currentAngle, 0);
            GameObject projectile = Instantiate(_projectilePrefab, _baseEmitter.position, Quaternion.identity);
            projectile.transform.forward = direction.normalized;
            if (projectile.TryGetComponent(out Orb orb))
            {
                orb.Launch(direction * _projectileSpeed);
                orb.CreateIndicator();
            }

            // Debug line to visualize the projectile direction
            Debug.DrawLine(_baseEmitter.position, _baseEmitter.position + direction.normalized * 2, Color.red, 5f);
        }

        // Optional delay after all projectiles are fired
        yield return new WaitForSeconds(delay);
    }



    public IEnumerator SymmetricalProjectile(int difficulty)
    {
        float difficultyPercent = difficulty / (float)_maxDifficulty;
        int n = (int)Mathf.Lerp(2, 4, difficultyPercent); // number of projectiles
        float delay = Mathf.Lerp(0.5f, 1.5f, 1f - difficultyPercent); // delay before each projectile
        float offset = _player.GetPredictedAngle((_player.rotateRadius / _projectileSpeed) + delay); // predict the player's angle when projectiles arrive
        offset = (offset + 360) % 360; // Normalize the angle to [0, 360)
        //Debug.Log("Eternatus: SymProjectile: PredictedAngle: " + offset);
        //Debug.DrawLine(baseEmitter.position, arcManager.GetPositionFromAngle(offset), Color.blue, 5f); // predicted angle
        yield return SymmetricalProjectile(n, offset, delay);
    }

    // ---------------------------------- Symmetrical Projectile Instant ----------------------------------
    public IEnumerator SymmetricalProjectileInstant(int n, float angleOffset, float initialDelay)
    {
        //model.rotation = Quaternion.Lerp(model.rotation, Quaternion.Euler(model.eulerAngles.x, currentAngle, model.eulerAngles.z), cnt / delay);
        // create an energy field with shaders maybe then spawn n projectiles symetrically.
        float elapsedTime = 0f;
        Quaternion targetRotation = transform.rotation;
        float angularDistance = Quaternion.Angle(_model.rotation, targetRotation); // Angular distance between models current and target rotation
        float rotationSpeed = 5 * angularDistance / initialDelay; // v = x/t, 5 is arbitrary
        rotationSpeed = Mathf.Lerp(rotationSpeed, rotationSpeed * 5, _difficulty / _maxDifficulty); // increase rotation speed with difficulty
        while (elapsedTime < initialDelay)
        {
            elapsedTime += Time.deltaTime;
            // Smoothly rotate the model towards the target angle
            _model.rotation = Quaternion.RotateTowards(_model.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
        // 360/ n = angle between each projectile, angleOffset = offset from the baseEmitter's forward direction

        float angleStep = 360f / n; // angle between each projectile
        angleOffset = (angleOffset + 360) % 360;
        
        _animator?.ResetTrigger("Attack");
        _animator?.SetTrigger("Attack");
        AudioManager.Instance.KillSFX(_attackClip);
        AudioManager.Instance.PlaySFX(_attackClip, 0f, 0.6f);
        for (int i = 0; i < n; i++)
        {
            //yield return new WaitForSeconds(delay);
            float currentAngle = i * angleStep + angleOffset; // angle of the current projectile
            currentAngle = (currentAngle + 360) % 360; // Normalize the angle to [0, 360)
            Vector3 direction = _arcManager.GetPositionFromAngle(currentAngle).normalized;
            //Debug.DrawLine(baseEmitter.position, arcManager.GetPositionFromAngle(currentAngle), Color.black, 5f);
            //Debug.Log("direction: "+ Mathf.Atan2(direction.z - arcManager.transform.position.z, direction.x- arcManager.transform.position.x) * Mathf.Rad2Deg);
            //Debug.Log("currentAngle: " + currentAngle);
            GameObject projectile = Instantiate(_projectilePrefab, _baseEmitter.position, Quaternion.identity);
            projectile.transform.forward = direction.normalized;
            if (projectile.TryGetComponent(out Orb orb))
            {
                orb.Launch(direction * _projectileSpeed);
                orb.CreateIndicator();
            }
            //Debug.Log(direction.normalized * projectileSpeed);

            Debug.DrawLine(_baseEmitter.position, _baseEmitter.position + direction.normalized * 2, Color.red, 5f);

        }

    }

    public IEnumerator SymmetricalProjectileInstant(int difficulty)
    {
        float difficultyPercent = difficulty / (float)_maxDifficulty;
        int n = (int)Mathf.Lerp(2, 6, difficultyPercent);
        float initialDelay = Mathf.Lerp(0.25f, 1f, 1f - difficultyPercent);
        float offset = _player.GetPredictedAngle((_player.rotateRadius / _projectileSpeed) + initialDelay);
        Debug.DrawLine(_baseEmitter.position, _arcManager.GetPositionFromAngle(offset), Color.blue, 5f);
        yield return SymmetricalProjectileInstant(3, offset, initialDelay);
    }


    // ---------------------------------- Area Attack ----------------------------------
    public IEnumerator AreaAttack(int n, float angleOffset, float arcAngle, float waitTime, float activeTime)
    {
        // create an indicator for each angle arc, wait for waitTime then activate the attack for activeTime
        // maybe electrify the ground or something when active

        yield return null;
        _animator?.ResetTrigger("Attack");
        _animator?.SetTrigger("Attack");
        //Debug.Log("Animator triggered");
        
        float angleStep = 360f / n; // angle between each projectile

        for (int i = 0; i < n; i++)
        {
            float currentAngle = i * angleStep + angleOffset; // angle 
            currentAngle = (currentAngle + 360) % 360; // Normalize the angle to [0, 360)
            Vector3 direction = _arcManager.GetPositionFromAngle(currentAngle);
            direction = direction.normalized;
            // instantiate an area attack 
            AreaAttack areaAttack = Instantiate(_areaAttackPrefab, _baseEmitter.position, Quaternion.identity).GetComponent<AreaAttack>();
            areaAttack.Activate(direction, arcAngle, waitTime, activeTime);

            Debug.DrawLine(_baseEmitter.position, _baseEmitter.position + direction.normalized * 2, Color.red, 5f);

        }

        yield return new WaitForSeconds(waitTime + activeTime);
    }
    public IEnumerator AreaAttack(int difficulty)
    {
        float difficultyPercent = difficulty / (float)_maxDifficulty;
        float pocketAngle = Mathf.Lerp(120, 40, difficultyPercent);

        int n = Mathf.Clamp(Mathf.FloorToInt(360f / (pocketAngle + 30f)), 1, 5);
        float arcAngle = (360f - (pocketAngle * n)) / n;


        float waitTime = Mathf.Lerp(3f, 4f, 1f - difficultyPercent);
        float activeTime = Mathf.Lerp(2f, 4f, difficultyPercent);
        float angleOffset = _player.GetPredictedAngle((waitTime));
        //Debug.Log("safe: " + safeAngle + " n: " + n + " arcAngle: " + arcAngle + " waitTime: " + waitTime + " activeTime: " + activeTime);
        Debug.DrawLine(_baseEmitter.position, _arcManager.GetPositionFromAngle(angleOffset), Color.blue, 5f);
        yield return AreaAttack(n, angleOffset, arcAngle, waitTime, activeTime);
    }

    // ---------------------------------- Projectile Instant ----------------------------------

    public IEnumerator ProjectileInstant(int n, float delay)
    {

        float elapsedTime = 0f;
        // TODO: set target to directions average



        Quaternion targetRotation = Quaternion.LookRotation(_arcManager.GetPositionFromAngle(_arcManager.GetAngleFromPosition((_player.GetFollowerPosition()))).normalized); // Target rotation for the model
        

        float angularDistance = Quaternion.Angle(_model.rotation, targetRotation); // Angular distance between models current and target rotation
        float rotationSpeed = 5 * angularDistance / delay; // v = x/t, 5 is arbitrary
        rotationSpeed = Mathf.Lerp(rotationSpeed, rotationSpeed * 5, _difficulty / _maxDifficulty); // increase rotation speed with difficulty
        while (elapsedTime < delay)
        {
            elapsedTime += Time.deltaTime;
            // Smoothly rotate the model towards the target angle
            _model.rotation = Quaternion.RotateTowards(_model.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }

        // Instantly shoot projectiles in the given directions
        _animator?.ResetTrigger("Attack");
        _animator?.SetTrigger("Attack");

        AudioManager.Instance.KillSFX(_attackClip);
        AudioManager.Instance.PlaySFX(_attackClip, 0f, 0.6f);
        Vector3[] directions = new Vector3[n];
        directions[0] = (_player.transform.position - _baseEmitter.position).normalized; // direction to player
        directions[1] = (_arcManager.GetPositionFromAngle(_player.GetPredictedAngle((_player.rotateRadius / _projectileSpeed))) - _baseEmitter.position).normalized; // direction to predicted
        directions[2] = (_player.GetFollowerPosition() - _baseEmitter.position).normalized; // direction to followerPos
        for (int i = 3; i < n; i++)
        {
            // random directions y = 0,
            directions[i] = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        }
        for (int i = 0; i < directions.Length; i++)
        {
            GameObject projectile = Instantiate(_projectilePrefab, _baseEmitter.position, Quaternion.identity);
            projectile.transform.forward = directions[i].normalized;
            if (projectile.TryGetComponent(out Orb orb))
            {
                orb.Launch(directions[i] * _projectileSpeed);
                orb.CreateIndicator();
            }
            Debug.DrawLine(_baseEmitter.position, _baseEmitter.position + directions[i].normalized * 2, Color.red, 5f);
        }
        
        yield return new WaitForSeconds(delay);
    }
    
    public IEnumerator ProjectileInstant(int difficulty)
    {
        yield return null;
        // not symmetrical
        // one to player, one to predicted, one to followerPos, rest is random
        int n = Mathf.Clamp(3 + difficulty / 5, 2, 5);
        float delay = Mathf.Lerp(0.5f, 1.5f, 1f - difficulty / _maxDifficulty);
        

        yield return ProjectileInstant(n, delay);
    }
}
