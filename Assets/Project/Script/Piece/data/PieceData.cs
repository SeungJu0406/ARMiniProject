using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King, Size, Null}

[CreateAssetMenu(menuName = "Piece")]
public class PieceData : ScriptableObject
{
    [SerializeField] public PieceType type;
}
 