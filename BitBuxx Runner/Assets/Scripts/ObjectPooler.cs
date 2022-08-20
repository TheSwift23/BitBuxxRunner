using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    public bool MutualizeWaitingPools = false;

    public bool NestWaitingPool = true;

    [Condition("NestWaitingPool", true)]
    public bool NestUnderThis = false;

    private GameObject _waitingPool = null;
    private ObjectPool _objectPool;
    private const int _initialPoolsListCapacity = 5;

    public static List<ObjectPool> _pools = new List<ObjectPool>(_initialPoolsListCapacity);

    public static void AddPool(ObjectPool pool)
    {
        if(_pools == null)
        {
            _pools = new List<ObjectPool>(_initialPoolsListCapacity);
        }
        if (!_pools.Contains(pool))
        {
            _pools.Add(pool); 
        }
    }

    public static void RemovePool(ObjectPool pool)
    {
        _pools?.Remove(pool); 
    }

    private void Awake()
    {
        Instance = this;
        FillObjectPool(); 
    }

    private void OnDestroy()
    {
        if ((_objectPool != null) && NestUnderThis)
        {
            RemovePool(_objectPool);
        }
    }

    protected virtual bool CreateWaitingPool()
    {
        if (!MutualizeWaitingPools)
        {
            // we create a container that will hold all the instances we create
            _waitingPool = new GameObject(DetermineObjectPoolName());
            SceneManager.MoveGameObjectToScene(_waitingPool, this.gameObject.scene);
            _objectPool = _waitingPool.AddComponent<ObjectPool>();
            _objectPool.PooledGameObjects = new List<GameObject>();
            ApplyNesting();
            return true;
        }
        else
        {
            ObjectPool objectPool = ExistingPool(DetermineObjectPoolName());
            if (objectPool != null)
            {
                _objectPool = objectPool;
                _waitingPool = objectPool.gameObject;
                return false;
            }
            else
            {
                _waitingPool = new GameObject(DetermineObjectPoolName());
                SceneManager.MoveGameObjectToScene(_waitingPool, this.gameObject.scene);
                _objectPool = _waitingPool.AddComponent<ObjectPool>();
                _objectPool.PooledGameObjects = new List<GameObject>();
                ApplyNesting();
                AddPool(_objectPool);
                return true;
            }
        }
    }

    public ObjectPool ExistingPool(string poolName)
    {
        if (_pools == null)
        {
            _pools = new List<ObjectPool>(_initialPoolsListCapacity);
        }
        if (_pools.Count == 0)
        {
            var pools = FindObjectsOfType<ObjectPool>();
            if (pools.Length > 0)
            {
                _pools.AddRange(pools);
            }
        }
        foreach (ObjectPool pool in _pools)
        {
            if ((pool != null) && (pool.name == poolName) && (pool.gameObject.scene == this.gameObject.scene))
            {
                return pool;
            }
        }
        return null;
    }

    private void ApplyNesting()
    {
        if (NestWaitingPool && NestUnderThis && (_waitingPool != null))
        {
            _waitingPool.transform.SetParent(this.transform);
        }
    }

    private string DetermineObjectPoolName()
    {
        return ("[ObjectPooler] " + this.name); 
    }

    public void FillObjectPool()
    {
        return; 
    }

    public GameObject GetPooledGameObject()
    {
        return null; 
    }

    public void DestroyObjectPool()
    {
        if (_waitingPool != null)
        {
            Destroy(_waitingPool.gameObject);
        }
    }
}
