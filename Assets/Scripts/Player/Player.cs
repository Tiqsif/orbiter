using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform rotateAround;
    private Vector3 rotatePoint;
    [Range(1,20)] public int speed = 4;
    [HideInInspector] public float angularVelocity;
    public float rotateRadius = 15f;
    public float tiltAngle = -15f;
    public float tiltDuration = 0.5f;
    public Transform model;

    private bool isClockwise = false;
    private PlayerFX playerFX;
    private PlayerSFX playerSFX;
    public bool isDead = false;
    public bool canShoot = false;

    private float holdThreshold = 1f; // Seconds to qualify as a hold
    private float holdTime = 0;

    public delegate void OnPlayerShootBegin();
    public static OnPlayerShootBegin onPlayerShootBegin;

    private void OnEnable()
    {
        ArcManager.onPlayerHit += OnPlayerHit;
        RiskBar.onRiskBarFull += OnRiskBarFull;
    }

    private void OnDisable()
    {
        ArcManager.onPlayerHit -= OnPlayerHit;
        RiskBar.onRiskBarFull -= OnRiskBarFull;
    }
    private void Awake()
    {
        playerFX = GetComponent<PlayerFX>();
        playerSFX = GetComponent<PlayerSFX>();

        //Application.targetFrameRate = 30;
        
    }
    private void Start()
    {
        rotatePoint = rotateAround ? rotateAround.position : Vector3.zero;
        transform.position = rotatePoint - Vector3.forward * rotateRadius;
        StartCoroutine(TiltRoutine());
    }

    private void Update()
    {
        if (isDead) return;
        CheckHold();
        CheckTap();
        CheckRelease();

        Debug.DrawLine(rotatePoint, FindObjectOfType<ArcManager>().GetPositionFromAngle(GetPredictedAngle(0.25f)), Color.cyan);
        
    }
    void FixedUpdate()
    {
        if (isDead) return;
        float rotationDirection = isClockwise ? -1f : 1f;
        angularVelocity = 25f * speed * rotationDirection;
        float angleThisFrame = angularVelocity * Time.fixedDeltaTime;
        //Debug.Log($"Direction: {(isClockwise ? "Clockwise" : "Counterclockwise")}, Angle: {angleThisFrame}");
        //transform.RotateAround(rotatePoint, Vector3.up, angularVelocity * Time.fixedDeltaTime);
        CustomRotateAround(transform, rotatePoint, Vector3.up, angleThisFrame);
        //Debug.Log(FindAnyObjectByType<ArcManager>().GetAngleFromPosition(transform.position));
    }


    void CheckTap()
    {
        // Check if there's at least one touch on the screen
        if (Input.touchCount > 0)
        {
            // Get the first touch (you can loop through touches if needed)
            Touch touch = Input.GetTouch(0);

            // Check if the touch just began (a tap)
            if (touch.phase == TouchPhase.Began)
            {
                ExecuteTap();
            }
        }
    }
    void ExecuteTap()
    {
        holdTime = 0;

        //Debug.Log("Touch began at position: " + touch.position);
        isClockwise = !isClockwise;
        //model.Rotate(0, 180, 0);
        Debug.Log($"Direction changed to: {(isClockwise ? "Clockwise" : "Counterclockwise")}");
        playerFX.PlayDirtBurst();
        playerSFX.PlayChangeDirection();
        StopAllCoroutines();
        StartCoroutine(TiltRoutine());

    }

    void CheckHold()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                holdTime += Time.deltaTime; // Increment hold timer
                if (holdTime >= holdThreshold)
                {
                    ExecuteHold();
                }
            }
        }
    }

    void ExecuteHold()
    {
        if (canShoot)
        {
            Debug.Log("Shoot");
            onPlayerShootBegin?.Invoke();
            canShoot = false;
        }

    }

    void CheckRelease()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                ExecuteRelease();
            }
        }
    }

    void ExecuteRelease()
    {
        holdTime = 0;
    }
    IEnumerator TiltRoutine()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0f);
        
        float elapsedTime = 0f;
        while (elapsedTime <= tiltDuration)
        {
            float angle = Mathf.Lerp(0, tiltAngle * (isClockwise ? 1:-1), elapsedTime / tiltDuration);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angle);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    void OnPlayerHit()
    {
        isDead = true;
    }

    void OnRiskBarFull()
    {
        canShoot = true;
    }

    public float GetPredictedAngle(float secondsLater)
    {
        float angleAdditional = angularVelocity * secondsLater;

        float currentAngle = Mathf.Atan2(transform.position.z - rotatePoint.z, transform.position.x - rotatePoint.x) * Mathf.Rad2Deg;

        currentAngle = (currentAngle + 360) % 360;

        float predictedAngle = (currentAngle + angleAdditional) % 360;

        return predictedAngle;

    }

    public void CustomRotateAround(Transform target, Vector3 pivot, Vector3 axis, float angle)
    {
        // Calculate the direction from the pivot to the target
        Vector3 direction = target.position - pivot;
        float angleToPlayer = Mathf.Atan2(target.position.z - pivot.z, target.position.x - pivot.x) * Mathf.Rad2Deg;
        float newAngle = (angleToPlayer + angle) % 360;
        // Rotate the direction vector using the quaternion
        float angleInRadians = newAngle * Mathf.Deg2Rad;
        float x = pivot.x + rotateRadius * Mathf.Cos(angleInRadians);
        float z = pivot.z + rotateRadius * Mathf.Sin(angleInRadians);
        Vector3 rotatedDirection = new Vector3(x, direction.y, z);
        //Debug.Log($"Rotating around {pivot} by {angle} degrees.");
        //Debug.Log($"Angle to player: {angleToPlayer}");
        //Debug.DrawRay(pivot, direction, Color.red);
        //Debug.DrawRay(pivot, rotatedDirection, Color.green);
        // Update the target's position based on the rotated direction
        target.position = pivot + rotatedDirection;

        Quaternion baseRotation = Quaternion.LookRotation(rotatedDirection - direction);
        Quaternion tiltRotation = Quaternion.Euler(0, 0, transform.localEulerAngles.z);
        target.rotation = baseRotation * tiltRotation ;
    }
}
