using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(ObjectPooler))]
public class ObjectSpawner : Spawner
{
	public enum GapOrigins { Spawner, LastSpawnedObject }

	[Header("Gap between objects")]
	/// is the gap related to the last object or the spawner ?
	public GapOrigins GapOrigin = GapOrigins.Spawner;
	/// the minimum gap bewteen two spawned objects
	public Vector3 MinimumGap = new Vector3(1, 1, 1);
	/// the maximum gap between two spawned objects
	public Vector3 MaximumGap = new Vector3(1, 1, 1);
	[Space(10)]
	[Header("Y Clamp")]
	/// the minimum Y position we can spawn the object at
	public float MinimumYClamp;
	/// the maximum Y position we can spawn the object at
	public float MaximumYClamp;
	[Header("Z Clamp")]
	/// the minimum Z position we can spawn the object at
	public float MinimumZClamp;
	/// the maximum Z position we can spawn the object at
	public float MaximumZClamp;
	[Space(10)]
	[Header("Spawn angle")]
	/// if true, the spawned objects will be rotated towards the spawn direction
	public bool SpawnRotatedToDirection = true;

	private Transform _lastSpawnedTransform;
	private float _nextSpawnDistance;
	private Vector3 _gap = Vector3.zero;

    private void Start()
    {
		_objectPooler = GetComponent<ObjectPooler>(); 
    }

    private void Update()
    {
		CheckSpawn(); 
    }

	private void CheckSpawn()
    {
		if (OnlySpawnWhileGameInProgress)
		{
			if ((PlayerMotor.isGameStarted != true))
			{
				_lastSpawnedTransform = null;
				return;
			}
		}

		if (_lastSpawnedTransform == null)
		{
			DistanceSpawn(transform.position + Maths.RandomVector3(MinimumGap, MaximumGap));
			return;
		}
		else
		{
			if (!_lastSpawnedTransform.gameObject.activeInHierarchy)
			{
				DistanceSpawn(transform.position + Maths.RandomVector3(MinimumGap, MaximumGap));
				return;
			}
		}

		if (transform.InverseTransformPoint(_lastSpawnedTransform.position).x < -_nextSpawnDistance)
		{
			Vector3 spawnPosition = transform.position;
			DistanceSpawn(spawnPosition);
		}
	}

	private void DistanceSpawn(Vector3 spawnPosition)
    {
		// we spawn a gameobject at the location we've determined previously
		GameObject spawnedObject = Spawn(spawnPosition, false);

		// if the spawned object is null, we're gonna start again with a fresh spawn next time we get fresh objects.
		if (spawnedObject == null)
		{
			_lastSpawnedTransform = null;
			_nextSpawnDistance = UnityEngine.Random.Range(MinimumGap.x, MaximumGap.x);
			return;
		}

		// we need to have a poolableObject component for the distance spawner to work.
		if (spawnedObject.GetComponent<PoolableObject>() == null)
		{
			throw new Exception(gameObject.name + " is trying to spawn objects that don't have a PoolableObject component.");
		}

		// if we have a movingObject component, we rotate it towards movement if needed
		if (SpawnRotatedToDirection)
		{
			spawnedObject.transform.rotation *= transform.rotation;
		}
		// if this is a moving object, we tell it to move in the designated direction
		if (spawnedObject.GetComponent<MovingObject>() != null)
		{
			spawnedObject.GetComponent<MovingObject>().SetDirection(transform.rotation * Vector3.left);
		}

		// if we've already spawned at least one object, we'll reposition our new object according to that previous one
		if (_lastSpawnedTransform != null)
		{
			// we center our object on the spawner's position
			spawnedObject.transform.position = transform.position;

			// we determine the relative x distance between our spawner and the object.
			float xDistanceToLastSpawnedObject = transform.InverseTransformPoint(_lastSpawnedTransform.position).x;

			// we position the new object so that it's side by side with the previous one,
			// taking into account the width of the new object and the last one.
			spawnedObject.transform.position += transform.rotation
												* Vector3.right
												* (xDistanceToLastSpawnedObject
												+ _lastSpawnedTransform.GetComponent<MMPoolableObject>().Size.x / 2
												+ spawnedObject.GetComponent<MMPoolableObject>().Size.x / 2);

			// if gaps are relative to the spawner
			if (GapOrigin == GapOrigins.Spawner)
			{
				spawnedObject.transform.position += (transform.rotation * ClampedPosition(MMMaths.RandomVector3(MinimumGap, MaximumGap)));
			}
			else
			{
				//MMDebug.DebugLogTime("relative y pos : "+spawnedObject.transform.InverseTransformPoint(_lastSpawnedTransform.position).y);

				_gap.x = UnityEngine.Random.Range(MinimumGap.x, MaximumGap.x);
				_gap.y = spawnedObject.transform.InverseTransformPoint(_lastSpawnedTransform.position).y + UnityEngine.Random.Range(MinimumGap.y, MaximumGap.y);
				_gap.z = spawnedObject.transform.InverseTransformPoint(_lastSpawnedTransform.position).z + UnityEngine.Random.Range(MinimumGap.z, MaximumGap.z);

				spawnedObject.transform.Translate(_gap);

				spawnedObject.transform.position = (transform.rotation * ClampedPositionRelative(spawnedObject.transform.position, transform.position));
			}
		}
		else
		{
			// we center our object on the spawner's position
			spawnedObject.transform.position = transform.position;
			// if gaps are relative to the spawner
			spawnedObject.transform.position += (transform.rotation * ClampedPosition(MMMaths.RandomVector3(MinimumGap, MaximumGap)));
		}

		// if what we spawn is a moving object (it should usually be), we tell it to move to account for initial movement gap
		if (spawnedObject.GetComponent<MovingObject>() != null)
		{
			spawnedObject.GetComponent<MovingObject>().Move();
		}

		//we tell our object it's now completely spawned
		spawnedObject.GetComponent<MMPoolableObject>().TriggerOnSpawnComplete();
		foreach (Transform child in spawnedObject.transform)
		{
			if (child.gameObject.GetComponent<ReactivateOnSpawn>() != null)
			{
				child.gameObject.GetComponent<ReactivateOnSpawn>().Reactivate();
			}
		}

		// we determine after what distance we should try spawning our next object
		_nextSpawnDistance = spawnedObject.GetComponent<MMPoolableObject>().Size.x / 2;
		// we store our new object, which will now be the previously spawned object for our next spawn
		_lastSpawnedTransform = spawnedObject.transform;
	}
}
