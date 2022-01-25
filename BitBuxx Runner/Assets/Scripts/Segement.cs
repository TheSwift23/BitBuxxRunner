using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segement : MonoBehaviour
{
    [SerializeField] int SegId { set; get; }
    [SerializeField] bool transition;

    [SerializeField] int length;
    [SerializeField] int beginY1, beginY2, beginY3;
    [SerializeField] int endY1, endY2, endY3;

    private Piece[] pieces;

    private void Awake()
    {
        pieces = gameObject.GetComponentsInChildren<Piece>(); 
    }

    void Spawn()
    {
        gameObject.SetActive(true); 
    }

    void DeSpawn()
    {
        gameObject.SetActive(false); 
    }
}
