using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSpawning : MonoBehaviour
{
    private BoxCollider portalCollider;
    private MeshRenderer portalRender;
    private MoreMountains.InfiniteRunnerEngine.MovingObject movingObject;
    [SerializeField] GameObject objectSpawner; 
    [SerializeField] Transform portaloriginalTransform;
    [SerializeField] Canvas portalCanvas;
    // Start is called before the first frame update
    private void Awake()
    {
        portalCollider = GetComponent<BoxCollider>();
        portalRender = GetComponent<MeshRenderer>();    
        movingObject = GetComponent<MoreMountains.InfiniteRunnerEngine.MovingObject>();
    }

    void Start()
    {
        portalCollider.enabled = false;
        portalRender.enabled = false;
        movingObject.enabled = false; 
        portalCanvas.enabled = false;   
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GameManager.scoreToTeleport); 
        if(GameManager.scoreToTeleport >= 20)
        {
            BeginTeleport();    
        }

        if(PlayerMotor.Teleporting == true)
        {
            Teleport(); 
        }
    }

    void BeginTeleport()
    {
        PlayerMotor.IsTeleporting = true;
        GameManager.scoreToTeleport = 0;
        movingObject.enabled = true;
        portalCollider.enabled = true;
        portalRender.enabled = true;
    }

    void Teleport()
    {
        portalCanvas.enabled = true;
    }

    void EndTeleport()
    {
         
    }

}
