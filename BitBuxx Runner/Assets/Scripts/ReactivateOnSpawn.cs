using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactivateOnSpawn : MonoBehaviour
{
	public bool ShouldReactivate = true;

	public virtual void Reactivate()
	{
		if (ShouldReactivate)
		{
			gameObject.SetActive(true);
		}
	}
}
