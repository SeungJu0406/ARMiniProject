using System.ComponentModel;
using UnityEngine;


public struct BoardPos
{
    public int y;
    public int x;
}
public class ChessBoard : MonoBehaviour
{
    public static ChessBoard Instance;

    public PieceType[,] board;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        board = new PieceType[8, 8];

        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                board[y, x] = PieceType.Null;
            }
        }
    }

    public BoardPos TransWorldToTile(Vector3 pos)
    {
        BoardPos boardPos = new BoardPos();
        boardPos.y = (int)pos.z + 4;
        boardPos.x = (int)pos.x + 4;
        return boardPos;
    }

    public Vector3 TransTileToWorld(BoardPos boardPos)
    {
        Vector3 pos = new Vector3();
        pos.x = (int)boardPos.x - 4;
        pos.z = (int)boardPos.y - 4;
        return pos;
    }

    public void PlacePiece(PieceType type, BoardPos boardPos)
    {
        board[boardPos.y, boardPos.x] = type;
    }
    public void UnPlacePiece(BoardPos boardPos)
    {
        board[boardPos.y, boardPos.x] = PieceType.Null;
    }
    public bool CheckTileInBoard(BoardPos boardPos, out PieceType type)
    {
        type = PieceType.Null;

        if(0 <= boardPos.y && boardPos.y < board.GetLength(0))
        {
            if(0 <= boardPos.x && boardPos.x < board.GetLength(1))
            {
                type = board[boardPos.y, boardPos.x];
                return true;
            }
        }
        return false;
    }
    public void DebugBoard()
    {
        for (int i = board.GetLength(0)-1; i >= 0 ; i--)
        {
            Debug.Log($"[{board[i, 0]}][{board[i, 1]}][{board[i, 2]}][{board[i, 3]}][{board[i, 4]}][{board[i, 5]}][{board[i, 6]}][{board[i, 7]}]");
        }
    }
}
