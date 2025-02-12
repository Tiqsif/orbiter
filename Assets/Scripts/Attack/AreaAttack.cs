using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAttack : MonoBehaviour
{
    [SerializeField] private AudioClip _blipClip;
    [SerializeField] private AudioClip _particleClip;
    [SerializeField] private GameObject _particlePrefab;
    [SerializeField] private Material _materialWhite;
    [SerializeField] private Material _materialAttack;
    protected Vector3 _direction;
    protected bool _hasIndicator = false;
    protected AreaIndicator _indicator;
    private ArcManager _arcManager;
    private GameObject _particleHolder;
    private AudioSource _source;
    private void Awake()
    {
        _arcManager = FindObjectOfType<ArcManager>();
        _materialWhite = new Material(_materialWhite);
        _materialAttack = new Material(_materialAttack);

    }
    public void Activate(Vector3 direction, float arcAngle, float waitTime, float activeTime)
    {
        this._direction = direction;
        
        _hasIndicator = true;

        GameObject indicatorObj = new GameObject("Indicator");
        _indicator = indicatorObj.AddComponent<AreaIndicator>();
        _indicator.materialWhite = _materialWhite;
        _indicator.materialAttack = _materialAttack;
        _indicator.blipClip = _blipClip;
        _indicator.Create(_arcManager, direction, arcAngle);
        StartCoroutine(ActivateRoutine(arcAngle, waitTime, activeTime));

    }

    private IEnumerator ActivateRoutine(float arcAngle, float waitTime, float activeTime)
    {
        // create a visual indicator for the orb
        yield return StartCoroutine(_indicator.FlashRoutine(waitTime));

        _arcManager.CreateRiskZone(_direction, arcAngle+30, activeTime);

        _indicator.HitMode();
        ActivateParticles(arcAngle, activeTime);
        float elepsedTime = 0;
        while (elepsedTime < activeTime)
        {
            _arcManager.CheckAreaHit(_direction, arcAngle);
            elepsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(_source.gameObject);
        ClearIndicator();
        ClearParticles();
        Destroy(this.gameObject);
    }

    public void ActivateParticles(float arcAngle, float activeTime)
    {
        float particleAngle = 15f;
        int numParticles = (int)(arcAngle / particleAngle);
        // instantiate particle prefab for every particleAngle in the arcAngle
        // starting from the direction - arcAngle / 2 to direction + arcAngle / 2
        for (int i = 0; i < numParticles; i++)
        {
            Vector3 particleDirection = Quaternion.Euler(0, (-arcAngle / 2) + i * particleAngle + particleAngle/2, 0) * _direction;
            GameObject particle = Instantiate(_particlePrefab, _arcManager.transform.position, Quaternion.identity);
            if (_particleHolder == null)
            {
                _particleHolder = new GameObject("ParticleHolder");
                _particleHolder.transform.parent = this.transform;
            }
            particle.transform.parent = _particleHolder.transform;
            // apply the particledirection to the particle
            
            particle.transform.forward = particleDirection;
        }

        _source = AudioManager.Instance.PlaySFX(_particleClip, loop:true);
        _source.pitch += Random.Range(0.3f,0.5f);
        _source.loop = true;

    }

    public void ClearParticles()
    {
        Destroy(_particleHolder);
    }

    public void ClearIndicator()
    {
        _hasIndicator = false;
        // destroy the visual indicator maybe with an effect
        Destroy(_indicator.gameObject);
    }
}
