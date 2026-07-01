using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    //We are making this as singleton so that this can globally accessed 
    public static PoolManager Instance;

    // We will create a condition to check if we want this instance destroy on load or not
    //Use cases when we are instantiating sounds and you dont want to destroy on scene load we can make it through this.
    [SerializeField] private bool _addtoDontDestroyOnLoad = false;

    //We are creating empty gameObject so that the instantiated object be a child of the these objects according to the parents, to create the heirarchy clean
    private GameObject _emptyHolder;

    private static GameObject _gameObjectsEmpty;
    private static GameObject _particleSystemEmpty;
    private static GameObject _soundFXEmpty;
    private static GameObject _afterImagesEmpty;

    //
    private static Dictionary<GameObject, ObjectPool<GameObject>> _objectPools;
    private static Dictionary<GameObject, GameObject> _cloneToPrefabMap;

    //
    public enum PoolType
    {
        GameObjects,
        Particles,
        SoundFX,
        AfterImages
    }

    public static PoolType poolingType;

    private void Awake()
    {
        Instance = this;

        //Initialize the dictionaries
        _objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        _cloneToPrefabMap = new Dictionary<GameObject, GameObject>();

        SetUpEmptyObjects();
    }

    private void SetUpEmptyObjects()
    {
        // We are setting the emptyHolder(Object pool) as the parent for all the other object which will be acting as a parent too

        _emptyHolder = new GameObject("Object Pool");

        _gameObjectsEmpty = new GameObject("GameObjects");
        // We are saying take _gameObjectsEmpty transform and make parentTransform(_emptyHolder) its->(_gameObjectsEmpty) parent.
        _gameObjectsEmpty.transform.SetParent(_emptyHolder.transform);

        _particleSystemEmpty = new GameObject("Particles");
        _particleSystemEmpty.transform.SetParent(_emptyHolder.transform);

        _soundFXEmpty = new GameObject("Sound FX");
        _soundFXEmpty.transform.SetParent(_emptyHolder.transform);

        _afterImagesEmpty = new GameObject("After Images");
        _afterImagesEmpty.transform.SetParent(_emptyHolder.transform);

        // We will select which parent object do we need not to be destroyed while loading 
        if(_addtoDontDestroyOnLoad)
        {
            //For now i have selected soundFX to not be destroyed while loading scene but im gonna keep _addtoDontDestroyOnLoad to false as i have no requirement of it.
            DontDestroyOnLoad(_soundFXEmpty.transform.root);
        }
    }

    private static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        //ObjectPool is a generic class
        //This is constructur which we are passing the required paramaters which are all actions and the first one is Func<>
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            //As you can see we are using lambdaExpression here because Func<> doesnt takes any parameters in the method so that we are creating an inline method and call the CreateObjects method in it
            createFunc: () => CreateObjects(prefab, pos, rot, poolType),

            //These are action which doesnt return anything 
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObjecct,
            actionOnDestroy: OnDestroyObject
            );

        _objectPools.Add(prefab, pool);
    }

    private static GameObject CreateObjects(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        prefab.SetActive(false);

        GameObject obj = Instantiate(prefab, pos, rot);

        prefab.SetActive(true);

        GameObject parentObject = SetParentObject(poolType);
        obj.transform.SetParent(parentObject.transform);

        return obj;
    }

    private static void OnGetObject(GameObject obj)
    {
        //Optional Logic , This can be used when we get the object
    }

    private static void OnReleaseObjecct(GameObject obj)
    {
        obj.SetActive(false);
    }

    private static void OnDestroyObject(GameObject obj)
    {
        // We are removing the object from the CloneToPrefab dictionary if the object exists
        if(_cloneToPrefabMap.ContainsKey(obj))
        {
            _cloneToPrefabMap.Remove(obj);
        }
    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        switch(poolType)
        {
            case PoolType.GameObjects:
                return _gameObjectsEmpty;

            case PoolType.Particles:
                return _particleSystemEmpty;

            case PoolType.SoundFX:
                return _soundFXEmpty;

            case PoolType.AfterImages:
                return _afterImagesEmpty;

            default:
                return null;
        }
    }

    private static T SpawnObject<T>(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Object
    {
        //We are checking if the object is there in the dictionary, if not we are creating a new one and if its there we are just gonna get it directly
        if(!_objectPools.ContainsKey(objectToSpawn))
        {
            CreatePool(objectToSpawn, spawnPos, spawnRotation, poolType);
        }

        GameObject obj = _objectPools[objectToSpawn].Get();

        //so we are checking if the obj is not null then we are adding it to _cloneToPrefabMap dictionary
        if(obj != null)
        {
            if(!_cloneToPrefabMap.ContainsKey(obj))
            {
                _cloneToPrefabMap.Add(obj, objectToSpawn);
            }

            //and we are setting the position and rotation to the parameters that we pass when we spawnObjects
            obj.transform.position = spawnPos;
            obj.transform.rotation = spawnRotation;
            obj.SetActive(true);

            if(typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }

            T component = obj.GetComponent<T>();

            if(component == null)
            {
                Debug.LogError($"Object {objectToSpawn.name} doesnt have component of Type {typeof(T)}");
                return null;
            }

            return component;
        }

        return null;
    }

    //So now that we want to able to handle both componnents and game objects we are going to create 2 overload methods

    // This will actually take a generic type T so we dont have to pass a gameObject in the parameter
    //This will be handle the components

    public static T SpawnObject<T>(T typePrefab, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRotation, poolType);
    }

    //This will handle the gameObjects
    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects)
    {
        return SpawnObject<GameObject>(objectToSpawn, spawnPos, spawnRotation, poolType);
    }

    //This method will handle the objects to return to the stack and deactivate them (so that it can be reused when required )

    public static void ReturnObjectToPool(GameObject obj, PoolType poolType = PoolType.GameObjects)
    {
        if (_cloneToPrefabMap.TryGetValue(obj, out GameObject prefab))
        {
            GameObject parentObject = SetParentObject(poolType);

            if(obj.transform.parent != parentObject.transform)
            {
                //we will re-parent the object 
                obj.transform.SetParent(parentObject.transform);
            }

            if(_objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                pool.Release(obj);
            }
        }
        else
        {
            Debug.LogWarning("Trying to return an object that is not pooled: " + obj.name);
        }
    }
}
