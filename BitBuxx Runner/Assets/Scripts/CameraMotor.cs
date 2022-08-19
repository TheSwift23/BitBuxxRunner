using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    [SerializeField] Transform lookAt; // Player we are looking at 
    [SerializeField] Vector3 offset = new Vector3(0, 7.5f, -7f);
    public Vector3 rotation = new Vector3(35, 0, 0); 

    public bool IsMoving { set; get; }

    private void LateUpdate()
    {
        if (!IsMoving)
            return; 

        Vector3 desiredPosition = lookAt.position + offset;
        transform.position = Vector3.Lerp(transform.position,desiredPosition,0.1f);
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(rotation), 0.1f); 
    }
}
