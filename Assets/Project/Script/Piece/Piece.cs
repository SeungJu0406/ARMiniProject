using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    [SerializeField] public PieceData data;

    [SerializeField] public Team team;

    public bool isMove;

    protected PieceStruct piece = new PieceStruct();

    public bool[,] ableMoveBoard = new bool[8, 8];
    public List<BoardPos> ablePos = new List<BoardPos>(8);
    public List<BoardPos> warningPos = new List<BoardPos>(8);

    protected List<GameObject> ableTiles = new List<GameObject>(8);

    protected List<PieceStruct> cachingWarningPieces = new List<PieceStruct>();
    protected bool isCheckWarningAfter;

    protected virtual void Start()
    {
        List<Piece> pieces = team == Team.White ? ChessBoard.Instance.whitePieces : ChessBoard.Instance.blackPieces;
        pieces.Add(this);

        BoardPos pos = ChessBoard.Instance.TransWorldToTile(transform.position);
        piece = ChessBoard.GetPieceStruct(this);
        ChessBoard.Instance.PlacePiece(piece, pos);
    }

    public abstract void AddAbleTile();

    public virtual void CheckOnWarningTile()
    {
        if (ablePos.Count == 0) return;

        CheckType kingCheck = team == Team.White ? ChessBoard.Instance.whiteKingCheck : ChessBoard.Instance.blackKingCheck;
        if (kingCheck == CheckType.Check)
        {
            List<Piece> canDefendPiece = team == Team.White ? ChessBoard.Instance.canDefendWhitePiece : ChessBoard.Instance.canDefendBlackPiece;
            // ������ ��� �Ұ��� �⹰�̸� ��� ableMoveBoard�� ablePos ����
            if (canDefendPiece.Contains(this) == false)
            {
                ClearAbleTile();
            }

        }
        else if (kingCheck == CheckType.Double)
        {
            // ŷ�� ��� ���� ������ ableMoveBoard�� ablePos ����
            ClearAbleTile();
        }



        // ���� ����Ʈ ���� �⹰�� �̵� �� ��
        // �� �̵� ����Ʈ�� ���ؼ� ��������Ʈ�� ������ �⹰�鿡 ���� �翬��
        // �̵� ����� able�Ǹ� �ش� Ÿ�� ����
        if (!isCheckWarningAfter)           // �ݺ� ������ ���� ���� bool üũ
        {
            BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position); // ���� ��ġ ĳ��
            Piece king = team == Team.White ? ChessBoard.Instance.whiteKing : ChessBoard.Instance.blackKing; // ŷ �ν��Ͻ� ĳ��
            AttackTile[,] attackTiles = team == Team.White ? ChessBoard.Instance.whiteAttackTiles : ChessBoard.Instance.blackAttackTiles; // ���� Ÿ�� ĳ��

            if (attackTiles[curPos.y, curPos.x].warnings.Count > 0)
            {   
                BoardPos kingPos = ChessBoard.Instance.TransWorldToTile(king.transform.position); // ���� ��ġ ĳ��            
                foreach (Piece warningPiece in attackTiles[curPos.y, curPos.x].warnings)
                {
                    PieceStruct cachingWarningPiece = ChessBoard.GetPieceStruct(warningPiece);
                    cachingWarningPieces.Add(cachingWarningPiece); // ���� �⹰ ĳ��
                }
                ChessBoard.Instance.UnPlacePiece(curPos); // �ӽ÷� ���� ��ġ ����

                for (int i = ablePos.Count - 1; i >= 0; i--)
                {
                    BoardPos movePos = ablePos[i];

                    ChessBoard.Instance.CheckTileOnBoard(movePos, out PieceStruct otherPiece); // �ӽ÷� �̵��� ��ġ�� �⹰ ����
                    ChessBoard.Instance.PlacePiece(piece, movePos); // �ӽ÷� �̵��� ��ġ�� ��ġ

                    foreach (PieceStruct warningPiece in cachingWarningPieces)
                    {
                        warningPiece.piece.AddAbleTile();
                        // ���� ����� ŷ�� Ÿ���� able�̵Ǹ�
                        attackTiles = team == Team.White ? ChessBoard.Instance.whiteAttackTiles : ChessBoard.Instance.blackAttackTiles; // ���� ���� �ٽ� ĳ��

                        if (attackTiles[kingPos.y, kingPos.x].ables.Count > 0)
                        {
                            // �ش� ���� ��ġ�� movePos �� ���� ���� ���� (���� ���)
                            BoardPos warningPiecePos = ChessBoard.Instance.TransWorldToTile(warningPiece.piece.transform.position);
                            if (warningPiecePos.x == movePos.x && warningPiecePos.y == movePos.y)
                                continue;

                            // �ش� Ÿ�� ������ �Ұ�(����Ʈ���� ����)
                            ablePos.RemoveAt(i);
                            ableMoveBoard[movePos.y, movePos.x] = false;
                            break;
                        }
                    }
                    // ables Ŭ����
                    attackTiles[kingPos.y, kingPos.x].ables.Clear();
                    ChessBoard.Instance.PlacePiece(otherPiece ,movePos); // �ӽ� ��ġ �ٽ� ����
                }
                ChessBoard.Instance.PlacePiece(piece, curPos); // ���� ��ġ�� �� ��ġ

                cachingWarningPieces.Clear();
            }
        }

        isCheckWarningAfter = true;
        // �ش� ��ġ�� able Ÿ�� ����
        CreateAbleTile();
    }

    public void CreateAbleTile()
    {     
        for (int i = 0; i < ablePos.Count; i++)
        {       
            Vector3 pos = ChessBoard.Instance.TransTileToWorld(ablePos[i]);
            GameObject ableTile = AbleZonePool.Instance.GetPool(pos);
            ableTiles.Add(ableTile);
        }
    }
    public void RemoveAbleTile()
    {
        foreach (GameObject tile in ableTiles)
        {
            AbleZonePool.Instance.ReturnPool(tile);
        }
        ableTiles.Clear();
    }
    public void ClearAbleTile()
    {
        for (int y = 0; y < ableMoveBoard.GetLength(0); y++)
        {
            for (int x = 0; x < ableMoveBoard.GetLength(1); x++)
            {
                ableMoveBoard[y, x] = false;
            }
        }

        ablePos.Clear();
    }
    public bool CheckAbleTile(BoardPos boardPos)
    {
        return ableMoveBoard[boardPos.y, boardPos.x];
    }

    public void Die()
    {
        List<Piece> pieces = team == Team.White ? ChessBoard.Instance.whitePieces : ChessBoard.Instance.blackPieces;
        pieces.Remove(this);
        Destroy(gameObject);
    }

    protected bool ProcessAbleTile(BoardPos movePos)
    {
        isCheckWarningAfter = false;

        if (ChessBoard.Instance.CheckTileOnBoard(movePos, out PieceStruct otherPiece))
        {
            if (otherPiece.type == PieceType.Null)
            {
                // �ش� ��ġ�� AbleZone�� ���� �� ����Ʈ�� �߰�
                ChessBoard.Instance.AddAbleTile(piece, movePos);
                ableMoveBoard[movePos.y, movePos.x] = true;
                ablePos.Add(movePos);
                return true;
            }
            else if (otherPiece.team != Team.Null && otherPiece.team != team)
            {

                // �ٸ� ���̸� AbleZone�� ���� �� ����Ʈ�� �߰� �� bool �� false
                ChessBoard.Instance.AddAbleTile(piece, movePos);
                ableMoveBoard[movePos.y, movePos.x] = true;
                ablePos.Add(movePos);
            }
            else if (otherPiece.team != Team.Null && otherPiece.team == team)
            {
                // ���� ���̸� �̵��� �Ұ��������� ������ ����
                ChessBoard.Instance.AddAbleTile(piece, movePos);
            }
        }
        return false;
    }
}
