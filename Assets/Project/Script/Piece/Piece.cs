using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    [SerializeField] public PieceData data;

    [SerializeField] public Team team;

    public bool isMove;

    protected PieceStruct piece = new PieceStruct();

    protected bool[,] ableMoveBoard = new bool[8, 8];
    protected List<GameObject> ableTiles = new List<GameObject>(8);
    protected List<BoardPos> ablePos = new List<BoardPos>(8);
    protected List<BoardPos> warningPos = new List<BoardPos>(8);

    List<PieceStruct> cachingWarningPieces = new List<PieceStruct>();

    protected virtual void Start()
    {
        ChessBoard.Instance.pieces.Add(this);

        BoardPos pos = ChessBoard.Instance.TransWorldToTile(transform.position);
        piece = ChessBoard.GetPieceStruct(this);
        ChessBoard.Instance.PlacePiece(piece, pos);
    }

    public abstract void AddAbleTile();

    public virtual void CheckOnWarningTile()
    {
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position); // ���� ��ġ ĳ��
        // ���� ����Ʈ ���� �⹰�� �̵� �� ��
        // �� �̵� ����Ʈ�� ���ؼ� ��������Ʈ�� ������ �⹰�鿡 ���� �翬��
        // �̵� ����� able�Ǹ� �ش� Ÿ�� ����
        if (team == Team.White && ChessBoard.Instance.blackAttackTiles[curPos.y, curPos.x].warnings.Count > 0)
        {
            BoardPos kingPos = ChessBoard.Instance.TransWorldToTile(ChessBoard.Instance.whiteKing.transform.position); // ���� ��ġ ĳ��
            foreach (Piece warningPiece in ChessBoard.Instance.blackAttackTiles[curPos.y, curPos.x].warnings)
            {
                PieceStruct cachingWarningPiece = ChessBoard.GetPieceStruct(warningPiece);
                cachingWarningPieces.Add(cachingWarningPiece); // ���� �⹰ ĳ��
            }
            ChessBoard.Instance.UnPlacePiece(curPos); // �ӽ÷� ���� ��ġ ����

            for (int i = ablePos.Count - 1; i >= 0; i--)
            {
                BoardPos movePos = ablePos[i];
                ChessBoard.Instance.PlacePiece(piece, movePos); // �ӽ÷� �̵��� ��ġ�� ��ġ

                foreach (PieceStruct warningPiece in cachingWarningPieces)
                {
                    warningPiece.piece.AddAbleTile();
                    // ���� ����� ŷ�� Ÿ���� able�̵Ǹ�
                    if (ChessBoard.Instance.blackAttackTiles[kingPos.y, kingPos.x].ables.Count > 0)
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
                ChessBoard.Instance.blackAttackTiles[kingPos.y, kingPos.x].ables.Clear();
                ChessBoard.Instance.UnPlacePiece(movePos); // �ӽ� �̵� ��ġ ����
            }
            ChessBoard.Instance.PlacePiece(piece, curPos); // ���� ��ġ�� �� ��ġ

            cachingWarningPieces.Clear();
        }
        else if (team == Team.Black && ChessBoard.Instance.whiteAttackTiles[curPos.y, curPos.x].warnings.Count > 0)
        {
            BoardPos kingPos = ChessBoard.Instance.TransWorldToTile(ChessBoard.Instance.blackKing.transform.position); // ���� ��ġ ĳ��
            foreach (Piece warningPiece in ChessBoard.Instance.whiteAttackTiles[curPos.y, curPos.x].warnings)
            {
                PieceStruct cachingWarningPiece = ChessBoard.GetPieceStruct(warningPiece);
                cachingWarningPieces.Add(cachingWarningPiece); // ���� �⹰ ĳ��
            }
            ChessBoard.Instance.UnPlacePiece(curPos); // �ӽ÷� ���� ��ġ ����

            for (int i = ablePos.Count - 1; i >= 0; i--)
            {
                BoardPos movePos = ablePos[i];
                ChessBoard.Instance.PlacePiece(piece, movePos); // �ӽ÷� �̵��� ��ġ�� ��ġ

                foreach (PieceStruct warningPiece in cachingWarningPieces)
                {
                    warningPiece.piece.AddAbleTile();
                    // ���� ����� ŷ�� Ÿ���� able�̵Ǹ�
                    if (ChessBoard.Instance.whiteAttackTiles[kingPos.y, kingPos.x].ables.Count > 0)
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
                ChessBoard.Instance.whiteAttackTiles[kingPos.y, kingPos.x].ables.Clear();
                ChessBoard.Instance.UnPlacePiece(movePos); // �ӽ� �̵� ��ġ ����
            }
            ChessBoard.Instance.PlacePiece(piece, curPos); // ���� ��ġ�� �� ��ġ

            cachingWarningPieces.Clear();
        }

        // �ش� ��ġ�� able Ÿ�� ����
        CreateAbleTile();
    }

    public void CreateAbleTile()
    {
        Debug.Log($"Ÿ�� ����{ablePos.Count}");
        for (int i = 0; i < ablePos.Count; i++)
        {
            Debug.Log($"��ȣ {i}");
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
    }
    public void ClearAbleTiel()
    {
        for (int y = 0; y < ableMoveBoard.GetLength(0); y++)
        {
            for (int x = 0; x < ableMoveBoard.GetLength(1); x++)
            {
                ableMoveBoard[y, x] = false;
            }
        }
        ableTiles.Clear();
        ablePos.Clear();
    }
    public bool CheckAbleTile(BoardPos boardPos)
    {
        return ableMoveBoard[boardPos.y, boardPos.x];
    }

    public void Die()
    {
        ChessBoard.Instance.pieces.Remove(this);
        Destroy(gameObject);
    }

    protected bool ProcessAbleTile(BoardPos movePos)
    {
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
                if (movePos.y == 7 && movePos.x == 4)
                    Debug.Log(otherPiece.type);

                // �ٸ� ���̸� AbleZone�� ���� �� ����Ʈ�� �߰� �� bool �� false
                ChessBoard.Instance.AddAbleTile(piece, movePos);
                ableMoveBoard[movePos.y, movePos.x] = true;
                ablePos.Add(movePos);
            }
        }
        return false;
    }
}
