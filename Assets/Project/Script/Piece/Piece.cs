using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    [SerializeField] public PieceData data; 

    protected bool[,] ableMoveBoard = new bool[8, 8];

    protected List<GameObject> ableTiles = new List<GameObject>(8);
    private void Start()
    {
        BoardPos pos = ChessBoard.Instance.TransWorldToTile(transform.position);
        ChessBoard.Instance.PlacePiece(data.type, pos);
    }

    public abstract void CreateAbleTile();
    public void RemoveAbleTile()
    {
        for (int y = 0; y < ableMoveBoard.GetLength(0); y++)
        {
            for (int x = 0; x < ableMoveBoard.GetLength(1); x++)
            {
                ableMoveBoard[y, x] = false;
            }
        }

        foreach (GameObject tile in ableTiles)
        {
            AbleZonePool.Instance.ReturnPool(tile);
        }
        ableTiles.Clear();
    }
    public bool CheckAbleTile(BoardPos boardPos)
    {
        return ableMoveBoard[boardPos.y, boardPos.x];
    }
}
