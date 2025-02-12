using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform _rotateAround;
    private Vector3 _rotatePoint;
    [Range(1,20)] public int speed = 4;
    private float _angularVelocity;
    public float rotateRadius = 12.5f;
    private float _tiltAngle = -15f;
    private float _tiltDuration = 1f;
    private Transform _model;

    private bool _isClockwise = false;
    private PlayerFX _playerFX;
    private PlayerSFX _playerSFX;
    public bool isDead { get; private set; }
    private bool _canShoot = false;

    //private float holdThreshold = 0.5f; // Seconds to qualify as a hold
    //private float holdTime = 0;

    private Vector3 _followerPos;
    private Vector3 _followerVel = Vector3.zero;

    public delegate void OnPlayerShootBegin(float damage);
    public static OnPlayerShootBegin onPlayerShootBegin;

    PlayerInputActions _playerInputActions;
    private void OnEnable()
    {
        ArcManager.onPlayerHit += OnPlayerHit;
        RiskBar.onRiskBarFull += OnRiskBarFull;

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Tap.performed += ExecuteTap;
        _playerInputActions.Player.Hold.performed += ExecuteHold;
    }

    private void OnDisable()
    {
        ArcManager.onPlayerHit -= OnPlayerHit;
        RiskBar.onRiskBarFull -= OnRiskBarFull;

        _playerInputActions.Player.Disable();
        _playerInputActions.Player.Tap.performed -= ExecuteTap;
        _playerInputActions.Player.Hold.performed -= ExecuteHold;

    }
    private void Awake()
    {
        _playerFX = GetComponent<PlayerFX>();
        _playerSFX = GetComponent<PlayerSFX>();
        //Application.targetFrameRate = 30;
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_followerPos, 0.2f);
    }
    private void Start()
    {
        _rotatePoint = _rotateAround ? _rotateAround.position : Vector3.zero;
        transform.position = _rotatePoint - Vector3.forward * rotateRadius;
        _followerPos = transform.position;
        StartCoroutine(TiltRoutine());
    }

    private void Update()
    {
        if (isDead)
        {
            _playerInputActions.Player.Disable();
            return;
        }



        /*
        CheckHold();
        CheckTap();
        CheckRelease();
        */
        //Debug.DrawLine(rotatePoint, FindObjectOfType<ArcManager>().GetPositionFromAngle(GetPredictedAngle(0.25f)), Color.cyan);

    }
    void FixedUpdate()
    {
        if (isDead) return;
        float rotationDirection = _isClockwise ? -1f : 1f;
        _angularVelocity = 25f * speed * rotationDirection;
        float angleThisFrame = _angularVelocity * Time.fixedDeltaTime;
        //Debug.Log($"Direction: {(isClockwise ? "Clockwise" : "Counterclockwise")}, Angle: {angleThisFrame}");
        //transform.RotateAround(rotatePoint, Vector3.up, angularVelocity * Time.fixedDeltaTime);
        CustomRotateAround(transform, _rotatePoint, Vector3.up, angleThisFrame);
        //Debug.Log(FindAnyObjectByType<ArcManager>().GetAngleFromPosition(transform.position));

        UpdateFollowerPosition();
    }


    /*
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
        else if (Input.GetMouseButtonDown(0))
        {
            // Backup for tap detection (useful for testing and low frame rates)
            ExecuteTap();
        }
    }
    */
    void ExecuteTap(InputAction.CallbackContext callbackContext)
    {
        //holdTime = 0;

        //Debug.Log("Touch began at position: " + touch.position);
        _isClockwise = !_isClockwise;
        //model.Rotate(0, 180, 0);
        //Debug.Log($"Direction changed to: {(isClockwise ? "Clockwise" : "Counterclockwise")}");
        _playerFX.PlayDirtBurst();
        _playerSFX.PlayChangeDirection();
        StopAllCoroutines();
        StartCoroutine(TiltRoutine());

    }

    /*
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
    */
    void ExecuteHold(InputAction.CallbackContext callbackContext)
    {
        //Debug.Log("Hold");
        if (_canShoot)
        {
            //Debug.Log("Shoot");
            onPlayerShootBegin?.Invoke(10f); // hit damage
            _playerSFX.PlayShoot();
            _canShoot = false;
        }

    }
    /*
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
    */
    void ExecuteRelease()
    {
        //holdTime = 0;
    }
    IEnumerator TiltRoutine()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0f);
        
        float elapsedTime = 0f;
        while (elapsedTime <= _tiltDuration)
        {
            float angle = Mathf.Lerp(0, _tiltAngle * (_isClockwise ? 1:-1), elapsedTime / _tiltDuration);
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
        _canShoot = true;
    }

    public float GetPredictedAngle(float secondsLater)
    {
        float angleAdditional = _angularVelocity * secondsLater;

        float currentAngle = Mathf.Atan2(transform.position.z - _rotatePoint.z, transform.position.x - _rotatePoint.x) * Mathf.Rad2Deg;

        currentAngle = (currentAngle + 360) % 360;

        float predictedAngle = (currentAngle + angleAdditional) % 360;

        return predictedAngle;

    }

    private void CustomRotateAround(Transform target, Vector3 pivot, Vector3 axis, float angle)
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

    public Vector3 GetFollowerPosition()
    {
        return _followerPos;
    }

    private void UpdateFollowerPosition()
    {
        Vector3 targetPosition = transform.position;

        // Calculate the smooth movement with acceleration
        _followerPos = Vector3.SmoothDamp(_followerPos, targetPosition, ref _followerVel, 0.2f, 10, Time.deltaTime);
    }

    public void SetRotateValues(Transform rotateAroundTransform, float rotateRadius)
    {
        _rotateAround = rotateAroundTransform;
        this.rotateRadius = rotateRadius;
        _rotatePoint = _rotateAround.position;
    }



    /*

#if UNITY_EDITOR
    /// <summary>
    /// Disables rendering debug manager in Unity Editor
    /// https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@14.0/manual/Render-Pipeline-Debug-Window.html#how-to-access-the-rendering-debugger
    ///
    /// <br/><br/>
    ///
    /// Prevent usable hotkeys from opening the rendering debug manager:
    /// <br/>
    /// PC: Left Ctrl + Backspace
    /// <br/>
    /// Gamepad: L3 + R3 (press the left and right sticks)
    /// <br/>
    /// Mobile: Three-finger tap
    /// </summary>
    /// ReSharper disable once CheckNamespace
    public static class DisableRenderingDebugManager // renders Display Stats window
    {
        [UnityEditor.InitializeOnLoadMethod]
        private static void OnEditorLoaded()
        {
            UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
        }
    }
#endif
    */
}
