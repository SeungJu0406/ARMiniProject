using UnityEngine;

public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King, Size, Null }
public enum Team { Null, Black, White }

[CreateAssetMenu(menuName = "Piece")]
public class PieceData : ScriptableObject
{
    [SerializeField] public PieceType type;
}
