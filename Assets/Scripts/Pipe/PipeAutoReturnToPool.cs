using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeAutoReturnToPool : MonoBehaviour
{
    [SerializeField] private float _minTime = 2f;
    [SerializeField] private float _maxTime = 3.5f;
    private float _lifeTime;

    private void OnEnable()
    {
        StartCoroutine(ReturnBodyToPool());

        _lifeTime = Random.Range(_minTime, _maxTime);
    }

    private IEnumerator ReturnBodyToPool()
    {
        yield return new WaitForSeconds(_lifeTime);

        PoolManager.ReturnObjectToPool(this.gameObject, PoolManager.PoolType.GameObjects);
    }
}
