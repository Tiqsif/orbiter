using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class ArcManager : MonoBehaviour
{
    [SerializeField] private Transform _player;
    public float circleRadius = 12.5f;


    public delegate void OnPlayerHit();
    public static OnPlayerHit onPlayerHit;


    [SerializeField] private GameObject _riskZonePrefab;
    private List<RiskZone> riskZones = new List<RiskZone>();
    private List<PlayerShot> shotsOnGround = new List<PlayerShot>();

    private float _angleToPlayer;

    public delegate void OnRiskZoneHit();
    public static OnRiskZoneHit onRiskZoneHit;

    public delegate void OnPlayerShotPickup();
    public static OnPlayerShotPickup onPlayerShotPickup;

    private void Awake()
    {
        Orb.OnOrbArrived +=OnOrbArrived;
        _player = _player? _player : GameObject.FindWithTag("Player").transform;

        if (_player.TryGetComponent(out Player movement)) movement.SetRotateValues(transform, circleRadius);
    }
    private void OnDestroy()
    {
        Orb.OnOrbArrived -= OnOrbArrived;
    }
    void Start()
    {
        riskZones = new List<RiskZone>();
        shotsOnGround = new List<PlayerShot>();
    }

    private void Update()
    {
        if (_player) _angleToPlayer = GetAngleFromPosition(_player.position);

        CheckPlayerShotsOnGround();
    }

    void FixedUpdate()
    {
        CheckRiskZone();
    }

    private bool IsWithinArc(float targetAngle, float centerAngle, float arcWidth)
    {
        float halfArc = arcWidth / 2f;
        float startAngle = (centerAngle - halfArc + 360f) % 360f;
        float endAngle = (centerAngle + halfArc + 360f) % 360f;

        // Normalize target angle
        targetAngle = (targetAngle + 360f) % 360f;

        if (Mathf.Approximately(arcWidth, 360f))
        {
            // Special case: full circle
            return true;
        }

        if (startAngle < endAngle)
        {
            // Standard case: angle range does not cross 360
            return targetAngle >= startAngle && targetAngle <= endAngle;
        }
        else
        {
            // Wrap-around case: angle range crosses 360
            return targetAngle >= startAngle || targetAngle <= endAngle;
        }
    }

    private void OnOrbArrived(Orb orb)
    {
        //Debug.Log("Orb arrived!");
        float arcAngle = Mathf.Rad2Deg * orb.width / circleRadius;

        // Calculate angles to projectile and player
        float angleToProjectile = Mathf.Atan2(orb.transform.position.z - transform.position.z, orb.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        float angleToPlayer = Mathf.Atan2(_player.position.z - transform.position.z, _player.position.x - transform.position.x) * Mathf.Rad2Deg;

        // Normalize angles
        angleToProjectile = (angleToProjectile + 360) % 360;
        angleToPlayer = (angleToPlayer + 360) % 360;

        // Check if player is within arc segment
        if (IsWithinArc(angleToPlayer, angleToProjectile, arcAngle))
        {
            Debug.Log("Player is within hit area!");
            if (_player.TryGetComponent(out Player movement) && !movement.isInvincible)
            {
                onPlayerHit?.Invoke(); // comment this for invincibility
            }
            // Trigger hit or damage logic
        }
    }

    public void CheckAreaHit(Vector3 direction, float arcAngle)
    {
        // calculate angle to player
        // check if the player is between direction - arcAngle/2 and direction + arcAngle/2
        // if true, trigger hit or damage logic

        // Calculate angles to projectile and player
        float areaCenterAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        float angleToPlayer = Mathf.Atan2(_player.position.z - transform.position.z, _player.position.x - transform.position.x) * Mathf.Rad2Deg;

        // Normalize angles
        areaCenterAngle = (areaCenterAngle + 360) % 360;
        angleToPlayer = (angleToPlayer + 360) % 360;

        // Check if player is within arc segment
        if (IsWithinArc(angleToPlayer, areaCenterAngle, arcAngle))
        {
            Debug.Log("Player is within hit area!");
            if (_player.TryGetComponent(out Player movement) && !movement.isInvincible)
            {
                onPlayerHit?.Invoke(); // comment this for invincibility
            }
            // Trigger hit or damage logic
        }
    }

    [ContextMenu("Test Risk")]
    public void testrisk()
    {
        CreateRiskZone(-Vector3.forward, 90, 5);
    }
    public void CreateRiskZone(Vector3 direction, float arcAgle, float lifetime)
    {
        Vector3 spawnPosition = transform.position;
        spawnPosition.y += 0.05f;
        GameObject riskZone = Instantiate(_riskZonePrefab, spawnPosition, Quaternion.identity);
        RiskZone zone = riskZone.GetComponent<RiskZone>();
        zone.Create(direction, circleRadius, arcAgle, lifetime);
        riskZones.Add(zone);
        StartCoroutine(DestroyRiskZoneAfter(zone, lifetime));
    }

    public void AddPlayerShotOnGround(PlayerShot playerShot)
    {
        shotsOnGround.Add(playerShot);
    }
    private IEnumerator DestroyRiskZoneAfter(RiskZone zone, float wait)
    {
        yield return new WaitForSeconds(wait);
        riskZones.Remove(zone);
        Destroy(zone.gameObject);
    }
    private void CheckRiskZone()
    {
        //Debug.Log("ArcMan: angletoPlayer: " + angleToPlayer);
        for (int i = riskZones.Count - 1; i >= 0; i--) 
        {
            RiskZone zone = riskZones[i];
            float zoneAngle = Mathf.Atan2(zone.direction.z, zone.direction.x) * Mathf.Rad2Deg;
                Vector3 left = transform.position + new Vector3(Mathf.Cos((zoneAngle - zone.arcAngle / 2) * Mathf.Deg2Rad), 0, Mathf.Sin((zoneAngle - zone.arcAngle / 2) * Mathf.Deg2Rad)) * circleRadius;
                Vector3 right = transform.position + new Vector3(Mathf.Cos((zoneAngle + zone.arcAngle / 2) * Mathf.Deg2Rad), 0, Mathf.Sin((zoneAngle + zone.arcAngle / 2) * Mathf.Deg2Rad)) * circleRadius;
                //Debug.DrawLine(transform.position, left, Color.blue, 1f);
                //Debug.DrawLine(transform.position, right, Color.red, 1f);
            if (IsWithinArc(_angleToPlayer, zoneAngle, zone.arcAngle))
            {
                Debug.DrawLine(transform.position, _player.position, Color.green, 1f);
                onRiskZoneHit?.Invoke();
                // draw line at the left and the right of the zone
            }
        }

    }

    private void CheckPlayerShotsOnGround()
    {
        for (int i = shotsOnGround.Count - 1; i >= 0; i--)
        {
            PlayerShot shot = shotsOnGround[i];
            float angle = GetAngleFromPosition(shot.transform.position);
            if (IsWithinArc(angle, _angleToPlayer, 10))
            {
                onPlayerShotPickup?.Invoke();
                shotsOnGround.Remove(shot);
                Destroy(shot.gameObject);
            }
        }
    
    }

    public Vector3 GetPositionFromAngle(float angle)
    {
        return transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * circleRadius;

    }

    public float GetAngleFromPosition(Vector3 position)
    {
        return Mathf.Atan2(position.z - transform.position.z, position.x - transform.position.x) * Mathf.Rad2Deg;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetPositionFromAngle(0), 1);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(GetPositionFromAngle(90), 1);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(GetPositionFromAngle(180), 1);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(GetPositionFromAngle(270), 1);
    }
}
