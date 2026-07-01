using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeParticleScript : MonoBehaviour
{
    [Header("Particles Reference")]
    [SerializeField] private GameObject[] _pipePieces;

    [Header("pipe pieces attributes")]
    private Rigidbody2D _rb;
    public Coroutine _spawnPipeParticles;

    [Header("Values to for the scatterting effect for the particle")]
    [Range(-10, 10)]
    [SerializeField] private float _minRandomForce = -2f;
    [Range(-10, 10)]
    [SerializeField] private float _maxRandomForce = 2f;

    private Vector2 _force;
    private float _torqueForce;

    public void CallSpawnPipeParticles()
    {
        if(this.isActiveAndEnabled)
        {
            if(_spawnPipeParticles != null)
            {
                StopCoroutine(SpawnPipeParticles());
            }

            _spawnPipeParticles = StartCoroutine(SpawnPipeParticles());
        }
    }    

    private IEnumerator SpawnPipeParticles()
    {
        List<GameObject> pipesList = new List<GameObject>();

        foreach(var part in _pipePieces)
        {
            var prefabs = PoolManager.SpawnObject(part, transform.position, Quaternion.identity, PoolManager.PoolType.GameObjects);

            _rb = prefabs.GetComponent<Rigidbody2D>();

            if (_rb == null) continue;

            _rb.isKinematic = false;
            _rb.WakeUp();
            _rb.velocity = Vector2.zero;
            _rb.angularVelocity = 0f;


            _force = new Vector2(Random.Range(_minRandomForce, _maxRandomForce), Random.Range(_minRandomForce, _maxRandomForce));

            _torqueForce = Random.Range(_minRandomForce, _maxRandomForce);

            _rb.AddForce(_force, ForceMode2D.Impulse);
            _rb.AddTorque(_torqueForce, ForceMode2D.Impulse);

            pipesList.Add(prefabs);
        }

        yield return null;
    }
}
