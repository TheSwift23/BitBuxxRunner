using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : ObjectBounds
{
    public delegate void Events();
    public event Events OnSpawnComplete;

    [Header("Poolable Object")]
    /// The life time, in seconds, of the object. If set to 0 it'll live forever, if set to any positive value it'll be set inactive after that time.
    public float LifeTime = 0f;

    public void Destroy()
    {
        gameObject.SetActive(false); 
    }

    private void OnEnable()
    {
        Size = GetBounds().extents * 2;
        if (LifeTime > 0f)
        {
            Invoke("Destroy", LifeTime);
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    /// <summary>
    /// Triggers the on spawn complete event
    /// </summary>
    public void TriggerOnSpawnComplete()
    {
        OnSpawnComplete?.Invoke();
    }
}
