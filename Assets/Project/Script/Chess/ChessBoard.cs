using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public enum CheckType { None, Check, Double }

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

    [SerializeField] ChessController controller;

    public List<Piece> whitePieces = new List<Piece>(15); // 킹 제외
    public List<Piece> canDefendWhitePiece = new List<Piece>(15);

    public List<Piece> blackPieces = new List<Piece>(15); // 킹 제외
    public List<Piece> canDefendBlackPiece = new List<Piece>(15);


    public King blackKing;
    public CheckType blackKingCheck;
    public King whiteKing;
    public CheckType whiteKingCheck;

    public PieceStruct[,] board = new PieceStruct[8, 8];
    public AttackTile[,] whiteAttackTiles = new AttackTile[8, 8];
    public AttackTile[,] blackAttackTiles = new AttackTile[8, 8];

    [Space(10)]
    [SerializeField] GameObject checkTile;
    StringBuilder sb;
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
    private void Start()
    {
        sb = Manager.UI.sb;
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

        piece.piece?.OccurEventOnTile(boardPos);
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
        AttackTile[,] attackTiles = piece.team == Team.White ? blackAttackTiles : whiteAttackTiles;
        attackTiles[boardPos.y, boardPos.x].ables.AddLast(piece.piece);
    }
    public void RemoveAbleTile(PieceStruct piece, BoardPos boardPos)
    {
        AttackTile[,] attackTiles = piece.team == Team.White ? blackAttackTiles : whiteAttackTiles;
        attackTiles[boardPos.y, boardPos.x].ables.Remove(piece.piece);
    }

    public void AddWarningTile(PieceStruct piece, BoardPos boardPos)
    {
        AttackTile[,] attackTiles = piece.team == Team.White ? blackAttackTiles : whiteAttackTiles;
        attackTiles[boardPos.y, boardPos.x].warnings.AddLast(piece.piece);
    }
    public void RemoveWarningTile(PieceStruct piece, BoardPos boardPos)
    {
        AttackTile[,] attackTiles = piece.team == Team.White ? blackAttackTiles : whiteAttackTiles;
        attackTiles[boardPos.y, boardPos.x].warnings.Remove(piece.piece);
    }


    public void InitAttackTile()
    {
        canDefendWhitePiece.Clear();
        canDefendBlackPiece.Clear();
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
        foreach (Piece piece in whitePieces)
        {
            piece.ClearAbleTile();
            piece.AddAbleTile();
        }
        foreach (Piece piece in blackPieces)
        {
            piece.ClearAbleTile();
            piece.AddAbleTile();
        }
        // 킹 값 초기화
        whiteKing.ClearAbleTile();
        blackKing.ClearAbleTile();
        blackKing.AddAttackTile();
        whiteKing.AddAbleTile();
        blackKing.AddAbleTile();
        // 킹이 체크 당했는지 체크
        CheckKingCheck();
    }

    AttackTile[,] attackTiles;
    BoardPos kingPos;
    List<Piece> pieces;
    List<Piece> canDefendPiece;
    void CheckKingCheck()
    {
        BoardPos whiteKingPos = TransWorldToTile(whiteKing.transform.position);
        BoardPos blackKingPos = TransWorldToTile(blackKing.transform.position);

        for (int i = 0; i < 2; i++)
        {
            attackTiles = i == 0 ? whiteAttackTiles : blackAttackTiles;
            kingPos = i == 0 ? whiteKingPos : blackKingPos;
            pieces = i == 0 ? whitePieces : blackPieces;
            canDefendPiece = i == 0 ? canDefendWhitePiece : canDefendBlackPiece;


            if (attackTiles[kingPos.y, kingPos.x].ables.Count == 1)
            {
                if (i == 0)
                    whiteKingCheck = CheckType.Check;
                else
                    blackKingCheck = CheckType.Check;

                ProcessCheckCase();
                CheckCheckmate();

            }
            else if (attackTiles[kingPos.y, kingPos.x].ables.Count > 1)
            {
                if (i == 0)
                    whiteKingCheck = CheckType.Double;
                else
                    blackKingCheck = CheckType.Double;


                CheckCheckmate();
            }
            else
            {
                if (i == 0)
                    whiteKingCheck = CheckType.None;
                else
                    blackKingCheck = CheckType.None;

                CheckCheckmate();
            }
        }
    }

    List<BoardPos> tempAblePos = new List<BoardPos>();
    bool[,] tempAbleMoveBoard = new bool[8, 8];
    void ProcessCheckCase()
    {
        Piece attackPiece = attackTiles[kingPos.y, kingPos.x].ables.First();
        List<BoardPos> warningPosList = attackPiece.warningPos;
        foreach (Piece piece in pieces)
        {
            for (int j = piece.ablePos.Count - 1; j >= 0; j--)
            {
                bool canDefend = false;
                foreach (BoardPos warningPos in warningPosList)
                {
                    if (piece.ablePos[j].y == warningPos.y && piece.ablePos[j].x == warningPos.x)
                    {
                        if (!canDefend)
                        {
                            tempAbleMoveBoard[piece.ablePos[j].y, piece.ablePos[j].x] = true;
                            BoardPos tempBoardPos = new BoardPos();
                            tempBoardPos.y = piece.ablePos[j].y;
                            tempBoardPos.x = piece.ablePos[j].x;
                            tempAblePos.Add(tempBoardPos);

                            canDefendPiece.Add(piece);
                            canDefend = true;
                        }
                    }
                }
            }
            SetTempToPieceAblePos(piece.ableMoveBoard, piece.ablePos);
            ClearTemp();
        }
    }
    #region Temp 변수 관리
    void SetTempToPieceAblePos(bool[,] ableMoveBoard, List<BoardPos> ablePos)
    {
        for (int y = 0; y < ableMoveBoard.GetLength(0); y++)
        {
            for (int x = 0; x < ableMoveBoard.GetLength(1); x++)
            {
                ableMoveBoard[y, x] = tempAbleMoveBoard[y, x];
            }
        }
        ablePos.Clear();
        for (int i = 0; i < tempAblePos.Count; i++)
        {
            ablePos.Add(tempAblePos[i]);
        }
    }
    void ClearTemp()
    {
        for (int y = 0; y < tempAbleMoveBoard.GetLength(0); y++)
        {
            for (int x = 0; x < tempAbleMoveBoard.GetLength(1); x++)
            {
                tempAbleMoveBoard[y, x] = false;
            }
        }

        tempAblePos.Clear();
    }
    #endregion

    void CheckCheckmate()
    {

        Piece king = pieces == whitePieces ? whiteKing : blackKing;
        CheckType kingCheck = pieces == whitePieces ? whiteKingCheck : blackKingCheck;

        // 본인 턴 일 때
        if (controller.curTeam == king.team)
        {
            if (kingCheck != CheckType.None)
            {
                ShowCheckTile(king);
                king.CheckOnWarningTile();
                if (king.ablePos.Count == 0 && canDefendPiece.Count == 0)  // 방어 할 수 있는 기물이 없으면서 , 왕이 움직일 수 없고 체크 일때
                {
                    CheckMateGameOver();
                }
            }
            else if (kingCheck == CheckType.None && CheckStaleMate(pieces)) // 체크는 아니지만 모든 기물이 움직일 수 없을 때
            {
                king.CheckOnWarningTile();
                if (king.ablePos.Count == 0)
                {
                    StaleMateGameOver();
                }
            }
            else
            {
                HideCheckTile();
            }
        }

    }

    bool CheckStaleMate(List<Piece> pieces)
    {
        foreach (Piece piece in pieces) // 스테일메이트 조건 : 체크상태는 아니지만 모든 기물이 움직일 수 없을 때
        {
            if (piece.ablePos.Count > 0)
            {
                return false;
            }
        }
        return true;
    }

    void ShowCheckTile(Piece king)
    {
        checkTile.SetActive(true);
        checkTile.transform.position = king.transform.position;
    }
    void HideCheckTile()
    {
        checkTile.SetActive(false);
    }

    void CheckMateGameOver()
    {
        ChessController.Instance.canClick = false;
        Manager.UI.ShowGameOverUI();
        sb.Clear();
        sb = ChessController.Instance.curTeam == Team.White ? sb.Append($"Black Win") : sb.Append($"White Win");
        Manager.UI.UpdateGameOverText(sb);
    }
    void StaleMateGameOver()
    {
        ChessController.Instance.canClick = false;
        Manager.UI.ShowGameOverUI();
        sb.Clear();
        sb.Append("StaleMate Draw");
        Manager.UI.UpdateGameOverText(sb);
    }







    public void DebugBoard()
    {
        for (int i = board.GetLength(0) - 1; i >= 0; i--)
        {
            Debug.Log($"[{board[i, 0].team}{board[i, 0].type}][{board[i, 1].team}{board[i, 1].type}][{board[i, 2].team}{board[i, 2].type}][{board[i, 3].team}{board[i, 3].type}][{board[i, 4].team}{board[i, 4].type}][{board[i, 5].team}{board[i, 5].type}][{board[i, 6].team}{board[i, 6].type}][{board[i, 7].team}{board[i, 7].type}]");
        }
    }

    public void DebugAttackBoard(int n)
    {
        AttackTile[,] attackTiles = n == 0 ? whiteAttackTiles : blackAttackTiles;
        
        for(int i = attackTiles.GetLength(0) -1; i>= 0; i--)
        {
            Debug.Log($"[{attackTiles[i, 0].ables.Count}][{attackTiles[i, 1].ables.Count}][{attackTiles[i, 2].ables.Count}][{attackTiles[i, 3].ables.Count}][{attackTiles[i, 4].ables.Count}][{attackTiles[i, 5].ables.Count}][{attackTiles[i, 6].ables.Count}][{attackTiles[i, 7].ables.Count}]");
        }
    }
}
