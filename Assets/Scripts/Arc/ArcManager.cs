using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ArcManager : MonoBehaviour
{
    public Transform player;
    public float circleRadius = 10;
    private void Awake()
    {
        Orb.OnOrbArrived +=OnOrbArrived;
        player = player? player : GameObject.FindWithTag("Player").transform;
        if (player.TryGetComponent(out PlayerMovement movement)) movement.rotateAround = transform; movement.rotateRadius = circleRadius;
    }
    private void OnDestroy()
    {
        Orb.OnOrbArrived -= OnOrbArrived;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool IsWithinArc(float targetAngle, float centerAngle, float arcWidth)
    {
        float lowerBound = centerAngle - arcWidth / 2;
        float upperBound = centerAngle + arcWidth / 2;

        // Handle wrap-around at 0/360 degrees
        if (lowerBound < 0) lowerBound += 360;
        if (upperBound >= 360) upperBound -= 360;

        if (lowerBound < upperBound)
            return targetAngle >= lowerBound && targetAngle <= upperBound;
        else
            return targetAngle >= lowerBound || targetAngle <= upperBound;
    }

    private void OnOrbArrived(Orb orb)
    {
        //Debug.Log("Orb arrived!");
        float arcAngle = Mathf.Rad2Deg * orb.width / circleRadius;

        // Calculate angles to projectile and player
        float angleToProjectile = Mathf.Atan2(orb.transform.position.z - transform.position.z, orb.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        float angleToPlayer = Mathf.Atan2(player.position.z - transform.position.z, player.position.x - transform.position.x) * Mathf.Rad2Deg;

        // Normalize angles
        angleToProjectile = (angleToProjectile + 360) % 360;
        angleToPlayer = (angleToPlayer + 360) % 360;

        // Check if player is within arc segment
        if (IsWithinArc(angleToPlayer, angleToProjectile, arcAngle))
        {
            Debug.Log("Player is within hit area!");
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
        float angleToPlayer = Mathf.Atan2(player.position.z - transform.position.z, player.position.x - transform.position.x) * Mathf.Rad2Deg;

        // Normalize angles
        areaCenterAngle = (areaCenterAngle + 360) % 360;
        angleToPlayer = (angleToPlayer + 360) % 360;

        // Check if player is within arc segment
        if (IsWithinArc(angleToPlayer, areaCenterAngle, arcAngle))
        {
            Debug.Log("Player is within hit area!");
            // Trigger hit or damage logic
        }
    }
}
