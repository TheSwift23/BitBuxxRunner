using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    none = -1, 
    ramp = 0, 
    longBlock = 1, 
    jump = 2, 
    manHole = 3,
    slide = 4, 
    wallRunLeft = 5,
    wallRunRight = 6, 
}

public class Piece : MonoBehaviour
{
    public PieceType type;
    public int visualIndex; 
}
