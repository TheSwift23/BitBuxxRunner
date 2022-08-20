using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    public enum PossibleDirections { Left, Right, Up, Down, Forwards, Backwards };
    public enum WaysToDetermineBounds { Collider, Collider2D, Renderer, Undefined }

    public WaysToDetermineBounds BoundsBasedOn;

    public Vector3 Size { get; set; }

    public float ParllaxSpeed;

    public PossibleDirections ParallaxDirection;

    GameObject _clone;
    Vector3 _movement;
    Vector3 _initialPosition;
    Vector3 _newPosition;
    Vector3 _direction;
    float _width;

    private void Reset()
    {
        DefineBoundsChoice();
    }

    protected virtual void DefineBoundsChoice()
    {
        BoundsBasedOn = WaysToDetermineBounds.Undefined;
        if (GetComponent<Renderer>() != null)
        {
            BoundsBasedOn = WaysToDetermineBounds.Renderer;
        }
        if (GetComponent<Collider>() != null)
        {
            BoundsBasedOn = WaysToDetermineBounds.Collider;
        }
        if (GetComponent<Collider2D>() != null)
        {
            BoundsBasedOn = WaysToDetermineBounds.Collider2D;
        }
    }

    public Bounds GetBounds()
    {
        if (BoundsBasedOn == WaysToDetermineBounds.Renderer)
        {
            if (GetComponent<Renderer>() == null)
            {
                throw new System.Exception("The PoolableObject " + gameObject.name + " is set as having Renderer based bounds but no Renderer component can be found.");
            }
            return GetComponent<Renderer>().bounds;
        }

        if (BoundsBasedOn == WaysToDetermineBounds.Collider)
        {
            if (GetComponent<Collider>() == null)
            {
                throw new System.Exception("The PoolableObject " + gameObject.name + " is set as having Collider based bounds but no Collider component can be found.");
            }
            return GetComponent<Collider>().bounds;
        }

        if (BoundsBasedOn == WaysToDetermineBounds.Collider2D)
        {
            if (GetComponent<Collider2D>() == null)
            {
                throw new System.Exception("The PoolableObject " + gameObject.name + " is set as having Collider2D based bounds but no Collider2D component can be found.");
            }
            return GetComponent<Collider2D>().bounds;
        }

        return new Bounds(Vector3.zero, Vector3.zero);
    }

    void Start()
    {
        if (ParallaxDirection == PossibleDirections.Left || ParallaxDirection == PossibleDirections.Right)
        {
            _width = GetBounds().size.x;
            _newPosition = new Vector3(transform.position.x + _width, transform.position.y, transform.position.z);
        }
        if (ParallaxDirection == PossibleDirections.Up || ParallaxDirection == PossibleDirections.Down)
        {
            _width = GetBounds().size.y;
            _newPosition = new Vector3(transform.position.x, transform.position.y + _width, transform.position.z);
        }
        if (ParallaxDirection == PossibleDirections.Forwards || ParallaxDirection == PossibleDirections.Backwards)
        {
            _width = GetBounds().size.z;
            _newPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + _width);
        }

        switch (ParallaxDirection)
        {
            case PossibleDirections.Backwards:
                _direction = Vector3.back;
                break;
            case PossibleDirections.Forwards:
                _direction = Vector3.forward;
                break;
            case PossibleDirections.Down:
                _direction = Vector3.down;
                break;
            case PossibleDirections.Up:
                _direction = Vector3.up;
                break;
            case PossibleDirections.Left:
                _direction = Vector3.left;
                break;
            case PossibleDirections.Right:
                _direction = Vector3.right;
                break;
        }

        _initialPosition = transform.position;

        _clone = (GameObject)Instantiate(gameObject, _newPosition, transform.rotation);
        Parallax parallaxComponent = _clone.GetComponent<Parallax>();
        Destroy(parallaxComponent);

    }



    // Update is called once per frame
    void Update()
    {
        if (PlayerMotor.isGameStarted != true)
        {
            _movement = _direction * (ParllaxSpeed/10) * Time.deltaTime;
        }
        else
        {
            _movement = _direction * (ParllaxSpeed/10) * PlayerMotor.speed * Time.deltaTime;
            _clone.transform.Translate(_movement);
            transform.Translate(_movement);
        }

        if (ShouldResetPosition())
        {
            transform.Translate(-_direction * _width);
            _clone.transform.Translate(-_direction * _width);
        }
    }

    protected virtual bool ShouldResetPosition()
    {
        switch (ParallaxDirection)
        {
            case PossibleDirections.Backwards:
                if (transform.position.z + _width < _initialPosition.z) { return true; } else { return false; }
            case PossibleDirections.Forwards:
                if (transform.position.z - _width > _initialPosition.z) { return true; } else { return false; }
            case PossibleDirections.Down:
                if (transform.position.y + _width < _initialPosition.y) { return true; } else { return false; }
            case PossibleDirections.Up:
                if (transform.position.y - _width > _initialPosition.y) { return true; } else { return false; }
            case PossibleDirections.Left:
                if (transform.position.x + _width < _initialPosition.x) { return true; } else { return false; }
            case PossibleDirections.Right:
                if (transform.position.x - _width > _initialPosition.x) { return true; } else { return false; }
        }
        return false;
    }
}
