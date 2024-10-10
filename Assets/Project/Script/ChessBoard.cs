using System.Collections.Generic;
using UnityEngine;


public struct BoardPos
{
    public int y;
    public int x;
}
public struct PieceStruct
{
    public Piece piece;
    public PieceType type;
    public Team team;
}
public struct AttackTile
{
    public LinkedList<Piece> ables;
    public LinkedList<Piece> attacks;
}


public class ChessBoard : MonoBehaviour
{
    public static ChessBoard Instance;

    public List<Piece> pieces = new List<Piece>(32);

    public PieceStruct[,] board= new PieceStruct[8, 8];

    public AttackTile[,] blackAttackTiles = new AttackTile[8, 8];
    public AttackTile[,] whiteAttackTiles = new AttackTile[8, 8];

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        for(int y = 0; y <blackAttackTiles. GetLength(0); y++)
        {
            for(int x = 0; x < blackAttackTiles.GetLength(1); y++)
            {
                AttackTile black = new AttackTile();
                black.ables = new LinkedList<Piece>();
                black.attacks = new LinkedList<Piece>();
                blackAttackTiles[y,x] = black;
                AttackTile white = new AttackTile();
                white.ables = new LinkedList<Piece>();
                white.attacks = new LinkedList<Piece>();
                whiteAttackTiles[y, x] = white;
            }
        }
        

        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                board[y, x].type = PieceType.Null;
            }
        }
    }
    public static PieceStruct GetPieceStruct(Piece piece, Team team)
    {
        PieceStruct pieceStruct = new PieceStruct();
        pieceStruct.piece = piece;
        pieceStruct.type = piece.data.type; 
        pieceStruct.team = team;
        return pieceStruct;
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

    public void PlacePiece(PieceStruct piece, BoardPos boardPos)
    {
        board[boardPos.y, boardPos.x] = piece;
    }
    public void UnPlacePiece(BoardPos boardPos)
    {
        board[boardPos.y, boardPos.x].type = PieceType.Null;
        board[boardPos.y, boardPos.x].team = Team.Null;
    }
    public bool CheckTileOnBoard(BoardPos boardPos, out PieceStruct piece)
    {
        piece.piece = null;
        piece.type = PieceType.Null;
        piece.team = Team.Null;

        if (0 <= boardPos.y && boardPos.y < board.GetLength(0))
        {
            if (0 <= boardPos.x && boardPos.x < board.GetLength(1))
            {
                piece.piece = board[boardPos.y, boardPos.x].piece;
                piece.type = board[boardPos.y, boardPos.x].type;
                piece.team = board[boardPos.y, boardPos.x].team;
                return true;
            }
        }
        return false;
    }

    public void AddAbleTiles(PieceStruct piece, BoardPos boardPos)
    {
        if(piece.team == Team.Black)
        {
            blackAttackTiles[boardPos.y, boardPos.x].ables.AddLast(piece.piece);
        }
        else if (piece.team == Team.White)
        {
            whiteAttackTiles[boardPos.y, boardPos.x].ables.AddLast(piece.piece);
        }
    }

    public void InitAttackTile()
    {
        for (int y = 0; y < blackAttackTiles.GetLength(0); y++)
        {
            for (int x = 0; x < blackAttackTiles.GetLength(1); y++)
            {
                blackAttackTiles[y, x].ables.Clear();
                blackAttackTiles[y, x].attacks.Clear();
                whiteAttackTiles[y, x].ables.Clear();
                whiteAttackTiles[y, x].attacks.Clear();
            }
        }
        foreach(Piece piece in pieces)
        {
            
        }
    }




    public void DebugBoard()
    {
        for (int i = board.GetLength(0) - 1; i >= 0; i--)
        {
            Debug.Log($"[{board[i, 0].team}{board[i, 0].type}][{board[i, 1].team}{board[i, 1].type}][{board[i, 2].team}{board[i, 2].type}][{board[i, 3].team}{board[i, 3].type}][{board[i, 4].team}{board[i, 4].type}][{board[i, 5].team}{board[i, 5].type}][{board[i, 6].team}{board[i, 6].type}][{board[i, 7].team}{board[i, 7].type}]");
        }
    }
}
