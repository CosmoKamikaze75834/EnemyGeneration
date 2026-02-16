using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    public event Action<Enemy> LifeTimeEnded;

    private Rigidbody _rigidbody;

    private int _lifeTime = 5;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        StartCoroutine(WaitForLifeTimeEnd());
    }

    private void OnDisable()
    {
        StopCoroutine(WaitForLifeTimeEnd());
    }

    public void ResetPosition()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.rotation = Quaternion.identity;
    }

    private IEnumerator WaitForLifeTimeEnd()
    {
        WaitForSeconds _wait = new WaitForSeconds(_lifeTime);

        yield return _wait;

        LifeTimeEnded?.Invoke(this);
    }
}