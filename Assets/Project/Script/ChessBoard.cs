using System.Collections.Generic;
using UnityEngine;


public enum CheckType { None, Check, Double}

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
    public LinkedList<Piece> warnings;
}


public class ChessBoard : MonoBehaviour
{
    public static ChessBoard Instance;

    public List<Piece> pieces = new List<Piece>(30); // 킹 제외


    public Piece blackKing;
    public CheckType blackKingCheck;
    public Piece whiteKing;
    public CheckType whiteKingCheck;

    public PieceStruct[,] board = new PieceStruct[8, 8];
    public AttackTile[,] whiteAttackTiles = new AttackTile[8, 8];
    public AttackTile[,] blackAttackTiles = new AttackTile[8, 8];

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        for (int y = 0; y < whiteAttackTiles.GetLength(0); y++)
        {
            for (int x = 0; x < whiteAttackTiles.GetLength(1); x++)
            {
                AttackTile black = new AttackTile();
                black.ables = new LinkedList<Piece>();
                black.warnings = new LinkedList<Piece>();
                whiteAttackTiles[y, x] = black;
                AttackTile white = new AttackTile();
                white.ables = new LinkedList<Piece>();
                white.warnings = new LinkedList<Piece>();
                blackAttackTiles[y, x] = white;
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
    public static PieceStruct GetPieceStruct(Piece piece)
    {
        PieceStruct pieceStruct = new PieceStruct();
        pieceStruct.piece = piece;
        pieceStruct.type = piece.data.type;
        pieceStruct.team = piece.team;
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
        board[boardPos.y, boardPos.x].type = piece.type;
        board[boardPos.y, boardPos.x].team = piece.team;
    }
    public void UnPlacePiece(BoardPos boardPos)
    {
        board[boardPos.y, boardPos.x].piece = null;
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

    public void AddAbleTile(PieceStruct piece, BoardPos boardPos)
    {
        if (piece.team == Team.Black)
        {
            whiteAttackTiles[boardPos.y, boardPos.x].ables.AddLast(piece.piece);
        }
        else if (piece.team == Team.White)
        {
            blackAttackTiles[boardPos.y, boardPos.x].ables.AddLast(piece.piece);
        }
    }
    public void RemoveAbleTile(PieceStruct piece, BoardPos boardPos)
    {
        if (piece.team == Team.Black)
        {
            whiteAttackTiles[boardPos.y, boardPos.x].ables.Remove(piece.piece);
        }
        else if (piece.team == Team.White)
        {
            blackAttackTiles[boardPos.y, boardPos.x].ables.Remove(piece.piece);
        }
    }

    public void AddWarningTile(PieceStruct piece, BoardPos boardPos)
    {
        if (piece.team == Team.Black)
        {
            whiteAttackTiles[boardPos.y, boardPos.x].warnings.AddLast(piece.piece);
        }
        else if (piece.team == Team.White)
        {
            blackAttackTiles[boardPos.y, boardPos.x].warnings.AddLast(piece.piece);
        }
    }
    public void RemoveWarningTile(PieceStruct piece, BoardPos boardPos)
    {
        if (piece.team == Team.Black)
        {
            whiteAttackTiles[boardPos.y, boardPos.x].warnings.Remove(piece.piece);
        }
        else if (piece.team == Team.White)
        {
            blackAttackTiles[boardPos.y, boardPos.x].warnings.Remove(piece.piece);
        }
    }


    public void InitAttackTile()
    {
        for (int y = 0; y < whiteAttackTiles.GetLength(0); y++)
        {
            for (int x = 0; x < whiteAttackTiles.GetLength(1); x++)
            {
                whiteAttackTiles[y, x].ables.Clear();
                whiteAttackTiles[y, x].warnings.Clear();
                blackAttackTiles[y, x].ables.Clear();
                blackAttackTiles[y, x].warnings.Clear();
            }
        }
        foreach (Piece piece in pieces)
        {
            piece.ClearAbleTile();
            piece.AddAbleTile();       
        }
        whiteKing.ClearAbleTile();
        blackKing.ClearAbleTile();
        blackKing.AddAbleTile();
        whiteKing.AddAbleTile();
        // 킹이 체크 당했는지 체크
        CheckKingCheck();

    }

    void CheckKingCheck()
    {
        BoardPos whiteKingPos = TransWorldToTile(whiteKing.transform.position);
        BoardPos blackKingPos = TransWorldToTile(blackKing.transform.position);

        for (int i = 0; i < 2; i++)
        {
            AttackTile[,] attackTiles = i == 0 ? whiteAttackTiles : blackAttackTiles;
            BoardPos kingPos = i == 0 ? whiteKingPos : blackKingPos;
            CheckType kingCheck = i == 0 ? whiteKingCheck : blackKingCheck;

            if (attackTiles[kingPos.y, kingPos.x].ables.Count == 1)
            {
                kingCheck = CheckType.Check;
            }
            else if (attackTiles[kingPos.y, kingPos.x].ables.Count > 1)
            {
                kingCheck = CheckType.Double;
            }
            else
            {
                kingCheck = CheckType.None;
            }
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
