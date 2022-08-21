using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyInterface : MonoBehaviour
{
	private string _mode;
	private Rigidbody2D _rigidbody2D;
	private Rigidbody _rigidbody;
	private Collider2D _collider2D;
	private Collider _collider;
	private Bounds _colliderBounds;

	public Vector3 position
	{
		get
		{
			if (_rigidbody2D != null)
			{
				return _rigidbody2D.position;
			}
			if (_rigidbody != null)
			{
				return _rigidbody.position;
			}
			return Vector3.zero;
		}
		set { }
	}

	public Rigidbody2D InternalRigidBody2D
	{
		get
		{
			return _rigidbody2D;
		}
	}

	public Rigidbody InternalRigidBody
	{
		get
		{
			return _rigidbody;
		}
	}

	public Vector3 Velocity
	{
		get
		{
			if (_mode == "2D")
			{
				return (_rigidbody2D.velocity);
			}
			else
			{
				if (_mode == "3D")
				{
					return (_rigidbody.velocity);
				}
				else
				{
					return new Vector3(0, 0, 0);
				}
			}
		}
		set
		{
			if (_mode == "2D")
			{
				_rigidbody2D.velocity = value;
			}
			if (_mode == "3D")
			{
				_rigidbody.velocity = value;
			}
		}
	}

	public Bounds ColliderBounds
	{
		get
		{
			if (_rigidbody != null)
			{
				return _collider.bounds;
			}
			return new Bounds();
		}
	}

	public bool isKinematic
	{
		get
		{
			if (_mode == "2D")
			{
				return (_rigidbody2D.isKinematic);
			}
			if (_mode == "3D")
			{
				return (_rigidbody.isKinematic);
			}
			return false;
		}
	}

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();

		if (_rigidbody != null)
		{
			_mode = "3D";
			_collider = GetComponent<Collider>();
		}

	}

	public void AddForce(Vector3 force)
	{
		if (_mode == "3D")
		{
			_rigidbody.AddForce(force);
		}
	}

	public void AddRelativeForce(Vector3 force)
	{
		if (_mode == "2D")
		{
			_rigidbody2D.AddRelativeForce(force, ForceMode2D.Impulse);
		}
		if (_mode == "3D")
		{
			_rigidbody.AddRelativeForce(force);
		}
	}

	public void MovePosition(Vector3 newPosition)
	{
		if (_mode == "2D")
		{
			_rigidbody2D.MovePosition(newPosition);
		}
		if (_mode == "3D")
		{
			_rigidbody.MovePosition(newPosition);
		}
	}

	public void ResetAngularVelocity()
	{
		if (_mode == "2D")
		{
			_rigidbody2D.angularVelocity = 0;
		}
		if (_mode == "3D")
		{
			_rigidbody.angularVelocity = Vector3.zero;
		}
	}

	public void ResetRotation()
	{
		if (_mode == "2D")
		{
			_rigidbody2D.rotation = 0;
		}
		if (_mode == "3D")
		{
			_rigidbody.rotation = Quaternion.identity;
		}
	}

	public void IsKinematic(bool status)
	{
		if (_mode == "2D")
		{
			_rigidbody2D.isKinematic = status;
		}
		if (_mode == "3D")
		{
			_rigidbody.isKinematic = status;
		}
	}

	public void EnableBoxCollider(bool status)
	{
		if (_mode == "2D")
		{
			GetComponent<Collider2D>().enabled = status;
		}
		if (_mode == "3D")
		{
			GetComponent<Collider>().enabled = status;
		}
	}

	public bool Is3D
	{
		get
		{
			if (_mode == "3D")
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
    public void Update()
    {
		
    }
}
