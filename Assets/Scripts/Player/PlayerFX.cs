using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFX : MonoBehaviour
{
    [SerializeField] private Transform _dustEmitPoint;
    [SerializeField] private GameObject _dirtBurstPrefab;

    public void PlayDirtBurst()
    {
        GameObject dirtBurstObj = Instantiate(_dirtBurstPrefab, _dustEmitPoint.position, Quaternion.LookRotation(_dustEmitPoint.right), _dustEmitPoint);
    }
}
