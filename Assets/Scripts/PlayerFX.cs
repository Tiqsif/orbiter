using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFX : MonoBehaviour
{
    public Transform dustEmitPoint;
    public GameObject dirtBurst;

    public void PlayDirtBurst()
    {
        GameObject dirtBurstObj = Instantiate(dirtBurst, dustEmitPoint.position, Quaternion.LookRotation(dustEmitPoint.right), dustEmitPoint);
    }
}
