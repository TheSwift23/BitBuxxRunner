using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools; 

public class MoveBeginningObjects : MonoBehaviour
{
    private MoreMountains.InfiniteRunnerEngine.MovingObject movingObject; 
    // Start is called before the first frame update
    void Start()
    {
        movingObject = GetComponent<MoreMountains.InfiniteRunnerEngine.MovingObject>();
        movingObject.enabled = false; 
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerMotor.isGameStarted == true)
        {
            movingObject.enabled = true;
        }
        else
        {
            movingObject.enabled = false; 
        }
    }
}
