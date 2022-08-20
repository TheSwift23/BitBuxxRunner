using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 

[RequireComponent(typeof(ObjectPooler))]
public class Spawner : MonoBehaviour
{
	[Header("Size")]
	/// the minimum size of a spawned object
	public Vector3 MinimumSize = new Vector3(1, 1, 1);
	/// the maximum size of a spawned object
	public Vector3 MaximumSize = new Vector3(1, 1, 1);
	/// if set to true, the random size will preserve the original's aspect ratio
	public bool PreserveRatio = false;
	[Space(10)]
	[Header("Rotation")]
	/// the minimum size of a spawned object
	public Vector3 MinimumRotation;
	/// the maximum size of a spawned object
	public Vector3 MaximumRotation;
	[Space(10)]
	[Header("When can it spawn?")]
	/// if true, the spawner can spawn, if not, it won't spawn
	public bool Spawning = true;
	/// if true, only spawn objects while the game is in progress
	public bool OnlySpawnWhileGameInProgress = true;
	/// Initial delay before the first spawn, in seconds.
	public float InitialDelay = 0f;

	public ObjectPooler _objectPooler;
	private float _startTime;

	private void Awake()
	{
		_objectPooler = GetComponent<ObjectPooler>();
		_startTime = Time.time;
	}

	public GameObject Spawn(Vector3 spawnPosition, bool triggerObjectActivation = true)
    {
        if (OnlySpawnWhileGameInProgress)
        {
			if(PlayerMotor.isGameStarted != true)
            {
				return null; 
            }
        }
		if ((Time.time - _startTime < InitialDelay) || (!Spawning))
		{
			return null;
		}
		GameObject nextGameObject = _objectPooler.GetPooledGameObject();
		if (nextGameObject == null)
		{
			return null;
		}
		Vector3 newScale;
		if (!PreserveRatio)
		{
			newScale = new Vector3(UnityEngine.Random.Range(MinimumSize.x, MaximumSize.x), UnityEngine.Random.Range(MinimumSize.y, MaximumSize.y), UnityEngine.Random.Range(MinimumSize.z, MaximumSize.z));
		}
		else
		{
			newScale = Vector3.one * UnityEngine.Random.Range(MinimumSize.x, MaximumSize.x);
		}
		nextGameObject.transform.localScale = newScale;

		// we adjust the object's position based on its renderer's size
		if (nextGameObject.GetComponent<PoolableObject>() == null)
		{
			throw new Exception(gameObject.name + " is trying to spawn objects that don't have a PoolableObject component.");
		}

		nextGameObject.transform.position = spawnPosition;

		nextGameObject.transform.eulerAngles = new Vector3(
				UnityEngine.Random.Range(MinimumRotation.x, MaximumRotation.x),
				UnityEngine.Random.Range(MinimumRotation.y, MaximumRotation.y),
				UnityEngine.Random.Range(MinimumRotation.z, MaximumRotation.z)
				);

		nextGameObject.gameObject.SetActive(true);

		if (triggerObjectActivation)
		{
			if (nextGameObject.GetComponent<PoolableObject>() != null)
			{
				nextGameObject.GetComponent<PoolableObject>().TriggerOnSpawnComplete();
			}
			foreach (Transform child in nextGameObject.transform)
			{
				if (child.gameObject.GetComponent<ReactivateOnSpawn>() != null)
				{
					child.gameObject.GetComponent<ReactivateOnSpawn>().Reactivate();
				}
			}
		}

		return (nextGameObject); 

	}

}
