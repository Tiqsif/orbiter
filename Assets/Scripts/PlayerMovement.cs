using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform rotateAround;
    private Vector3 rotatePoint;
    [Range(1,20)] public int speed = 4;
    public float rotateRadius = 10f;
    public float tiltAngle = -15f;
    public float tiltDuration = 0.5f;
    public Transform model;

    private bool isClockwise = true;
    private PlayerFX playerFX;
    private PlayerSFX playerSFX;

    private void Awake()
    {
        playerFX = GetComponent<PlayerFX>();
        playerSFX = GetComponent<PlayerSFX>();
    }
    private void Start()
    {
        rotatePoint = rotateAround ? rotateAround.position : Vector3.zero;
        StartCoroutine(TiltRoutine());
    }
    void Update()
    {
        CheckTap();
        float rotationDirection = isClockwise ? 1f : -1f;
        transform.RotateAround(rotatePoint, Vector3.up, 25f * speed * Time.deltaTime * rotationDirection);
        // set the players rotation to a little tilted inwards




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
                Debug.Log("Touch began at position: " + touch.position);
                isClockwise = !isClockwise;
                model.Rotate(0, 180, 0);
                playerFX.PlayDirtBurst();
                playerSFX.PlayChangeDirection();
                StopAllCoroutines();
                StartCoroutine(TiltRoutine());
            }
        }
    }

    IEnumerator TiltRoutine()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0f);
        
        float elapsedTime = 0f;
        while (elapsedTime <= tiltDuration)
        {
            float angle = Mathf.Lerp(0, tiltAngle, elapsedTime / tiltDuration);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angle);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
