using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipesAndBirdsSpawnSystem : MonoBehaviour
{
    //Spawner Position
    private Vector3 _spawnerInitialPosition;

    [Header("Pipes")]
    [SerializeField] private GameObject _pipes;

    [Header("Distance between pipe initiation")]
    [SerializeField] private float _pipeSpacing = 5f;
    private float _lastPipeSpawnX;

    [Header("Offset in Y axis for pipe placement")]
    [SerializeField] private float _minOffset = -1.5f;
    [SerializeField] private float _maxOffset = 1.5f;

    [Header("Birds")]
    [SerializeField] private GameObject _birds;
    [SerializeField] private float _birdsSpawnCooldown = 10f;
    [SerializeField] private float _birdSpawnTime = 1f;
    private bool _spawningBird;

    [Header("Time between Bird initiation")]
    [SerializeField] private float _minBirdSpawnTime = 0.3f;
    [SerializeField] private float _maxBirdSpawnTime = 0.7f;

    [Header("Offset in Y axis for bird spawn point")]
    [SerializeField] private float _minOffsetBirdSpawn = -1.5f;
    [SerializeField] private float _maxOffsetBirdSpawn = 1.5f;

    private Coroutine _pipeCoroutine;
    private Coroutine _birdCoroutine;

    [Header("Pool for birds and Pipes")]
    private List<GameObject> _returnStorageForPipes = new List<GameObject>();
    private List<GameObject> _returnStorageForBirds = new List<GameObject>();

    private void Awake()
    {
        //_spawnerInitialPosition = Camera.main.transform.position + new Vector3(10,0,0);
    }

    private void Start()
    {
        //Debug.Log($"Last Pipe Spawn X Position: {_lastPipeSpawnX}");

        _pipeCoroutine = StartCoroutine(SpawnPipes());

        _birdCoroutine = StartCoroutine(SpawnBirds());
    }

    private void Update()
    {
        if(GameManager.Instance.isFlappyDead)
        {
            if(_pipeCoroutine != null)
            {
                StopCoroutine(_pipeCoroutine);
                _pipeCoroutine = null;
            }

            if (_birdCoroutine != null)
            {
                StopCoroutine(_birdCoroutine);
                _birdCoroutine = null;
            }
            
        }
    }
    // We are deprecating this method to spawn the pipes because it wasnt robust because of the time based spawning as the flappy bird has this dash mechanics which cannot be ideal for this spawning system as there will be inconsistent spawning of the pipe. we will implement distance beased spawning so that pipes spawni in the scene consistently.
    //private IEnumerator SpawnPipes()
    //{
    //    while (GameManager.Instance.gamestates != GameStates.GameStarts)
    //    {   
    //        yield return null;
    //    }

    //    while (true)
    //    {
    //        float y = Random.Range(_minOffset, _maxOffset);

    //        PoolManager.SpawnObject(_pipes, transform.position + new Vector3(0, y, 0), Quaternion.identity, PoolManager.PoolType.GameObjects);

    //        float timeBetweenSpawning = Random.Range(_minPipeSpawnTime, _maxPipeSpawnTime);

    //        yield return new WaitForSeconds(timeBetweenSpawning);
    //    }
    //}

    private IEnumerator SpawnPipes()
    {
        while(GameManager.Instance.gamestates != GameStates.GameStarts || GameManager.Instance.gamestates == GameStates.GameOver)
        {
            yield return null;
        }

        //we are storing the current position in lastPipeSpawnX
        _lastPipeSpawnX = transform.position.x;

        while (true)
        {
            while(transform.position.x >= _lastPipeSpawnX + _pipeSpacing)
            {
                float y = Random.Range(_minOffset, _maxOffset);

                Vector3 spawnPosition = new Vector3(_lastPipeSpawnX + _pipeSpacing, transform.position.y + y, transform.position.z);

                var pipes = PoolManager.SpawnObject(_pipes, spawnPosition, Quaternion.identity, PoolManager.PoolType.GameObjects);

                if(pipes.activeInHierarchy)
                {
                    _returnStorageForPipes.Add(pipes);
                }

                _lastPipeSpawnX += _pipeSpacing;
            }

            yield return null;
        }
    }


    private IEnumerator SpawnBirds()
    {
        while (GameManager.Instance.gamestates != GameStates.GameStarts || GameManager.Instance.gamestates == GameStates.GameOver)
        {
            yield return null;
        }

        while (true)
        {
           
            yield return new WaitForSeconds(_birdsSpawnCooldown);

            //Wave starts
            float endTime = Time.time + _birdSpawnTime;

            StopCoroutine(_pipeCoroutine);

            while (Time.time < endTime)
            {
                float y = Random.Range(_minOffsetBirdSpawn, _maxOffsetBirdSpawn);

                var birds = PoolManager.SpawnObject(_birds, transform.position + new Vector3(0, y, 0), Quaternion.identity);

                if (birds.activeInHierarchy)
                {
                    _returnStorageForBirds.Add(birds);
                }

                float timeBetweenSpawning = Random.Range(_minBirdSpawnTime, _maxBirdSpawnTime);

                yield return new WaitForSeconds(timeBetweenSpawning);
            }

            //As the bird spawn stops we want to update the latest position of the Spawner Position so that we can resume the spawning of pipe
            _lastPipeSpawnX = transform.position.x;

            _pipeCoroutine = StartCoroutine(SpawnPipes());
            //Wave Ends
        }
    }

    public void ResetSpawner()
    {
        // Set the current position to the default position
        //transform.position = _spawnerInitialPosition;

        //Also set the last spawn position to default
        _lastPipeSpawnX = transform.position.x;

        //Once we stop all the coroutine
        StopAllCoroutines();

        ReturnAllPipesToPool();
        ReturnAllBirdsToPool();

        //we then start the Coroutines again (not be worried when the gameState changes to gameIntro the pressedRetryButton(bool) becomes false so this resetSpawner method will be executed once)
        _pipeCoroutine = StartCoroutine(SpawnPipes());

        _birdCoroutine = StartCoroutine(SpawnBirds());
    }

    private void ReturnAllPipesToPool()
    {
        for(int i= _returnStorageForPipes.Count - 1; i >= 0; i--)
        {
            GameObject pipe = _returnStorageForPipes[i];

            if(pipe != null && pipe.activeInHierarchy)
            {
                PoolManager.ReturnObjectToPool(pipe, PoolManager.PoolType.GameObjects);
            }
        }

        _returnStorageForPipes.Clear();
    }

    private void ReturnAllBirdsToPool()
    {
        for (int i = _returnStorageForBirds.Count - 1; i >= 0; i--)
        {
            GameObject bird = _returnStorageForBirds[i];

            if (bird != null && bird.activeInHierarchy)
            {
                PoolManager.ReturnObjectToPool(bird, PoolManager.PoolType.GameObjects);
            }
        }

        _returnStorageForBirds.Clear();
    }
}
