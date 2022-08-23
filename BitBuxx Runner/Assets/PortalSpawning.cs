using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSpawning : MonoBehaviour
{
    private BoxCollider portalCollider;
    private MeshRenderer portalRender;
    // Start is called before the first frame update
    private void Awake()
    {
        portalCollider = GetComponent<BoxCollider>();
        portalRender = GetComponent<MeshRenderer>();    
    }

    void Start()
    {
        portalCollider.enabled = false;
        portalRender.enabled = false; 
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(GameManager.scoreToTeleport); 
        if(GameManager.scoreToTeleport >= 20)
        {
            GameManager.scoreToTeleport = 0;
        }
    }
}
