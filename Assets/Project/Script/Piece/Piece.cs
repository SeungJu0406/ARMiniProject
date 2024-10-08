using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] public PieceData data;


    private void Start()
    {
        BoardPos pos = ChessBoard.Instance.TransTilePoint(transform.position);
        ChessBoard.Instance.PlacePiece(data.type, pos);
    }
}
