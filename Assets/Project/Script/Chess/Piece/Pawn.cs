using System.Collections;
using UnityEngine;

public class Pawn : Piece
{
    Vector3[] dirs = new Vector3[3];
    public override void AddAbleTile()
    {
        // 1. ���� ��ġ�� ���� ��ġ�� ��ȯ
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position);

        warningPos.Add(curPos);
        ChessBoard.Instance.AddWarningTile(piece, curPos); // ���� ������ ��ġ�� ��������Ʈ�� ����

        // 2. �� �� �ִ� ���⿡ ���� üũ  
        // 2-1. ���� ��鿡 ���� ������ �ٸ���
        if (team == Team.Black)
        {
            dirs[0] = new Vector3(0, 0, 1);
            dirs[1] = new Vector3(-1, 0, 1);
            dirs[2] = new Vector3(1, 0, 1);
        }
        else if (team == Team.White)
        {
            dirs[0] = new Vector3(0, 0, -1);
            dirs[1] = new Vector3(-1, 0, -1);
            dirs[2] = new Vector3(1, 0, -1);
        }

        for (int i = 0; i < dirs.Length; i++)
        {
            BoardPos movePos = curPos;
            // ���� �̵��� ���       
            if (i == 0)
            {
                // ������ ���� �����ٸ� 2ĭ���� ������ �� �ִ�   
                int count;
                if (isMove)
                    count = 1;
                else
                    count = 2;
                for (int j = 0; j < count; j++)
                {
                    movePos.y += (int)dirs[i].z;
                    // 3. �ش� ��ġ�� ü���� ���̰� null���϶��� ���
                    // �տ� �⹰�� ���� ���� ������ �� �ִ�
                    if (ChessBoard.Instance.CheckTileOnBoard(movePos, out PieceStruct otherPiece))
                    {
                        if (otherPiece.type == PieceType.Null)
                        {
                            if (!ProcessAbleTile(movePos))
                                continue;
                        }
                        else
                            break;
                    }
                }
            }
            // �밢�� �̵��� ��� �ش� ��ġ�� ���� ���� ���� ������ �� �ִ�
            else
            {
                movePos.x += (int)dirs[i].x;
                movePos.y += (int)dirs[i].z;

                if (ChessBoard.Instance.CheckTileOnBoard(movePos, out PieceStruct otherPiece))
                {
                    ChessBoard.Instance.AddAbleTile(piece, movePos);
                    if (otherPiece.team != Team.Null && otherPiece.team != team)
                    {
                        if (!ProcessAbleTile(movePos))
                            continue;
                    }
                }
            }
        }
    }
    protected override bool ProcessAbleTile(BoardPos movePos)
    {
        isCheckWarningAfter = false;

        if (ChessBoard.Instance.CheckTileOnBoard(movePos, out PieceStruct otherPiece))
        {
            if (otherPiece.type == PieceType.Null)
            {
                // �ش� ��ġ�� AbleZone�� ���� �� ����Ʈ�� �߰�
                ableMoveBoard[movePos.y, movePos.x] = true;
                ablePos.Add(movePos);
                return true;
            }
            else if (otherPiece.team != Team.Null && otherPiece.team != team)
            {

                // �ٸ� ���̸� AbleZone�� ���� �� ����Ʈ�� �߰� �� bool �� false
                ableMoveBoard[movePos.y, movePos.x] = true;
                ablePos.Add(movePos);
            }
        }
        return false;
    }

    public override void OccurEventOnTile(BoardPos boardPos)
    {
        int posY = team == Team.White ? 0 : 7;
        if (boardPos.y == posY)
        {
            Promotion();
        }
    }
    PromotionUI promotionUI;
    Piece promotionPiece;
    bool isPromotionChoise;
    void Promotion()
    {
        // ���θ�� UI ����
        StartCoroutine(PromotionRoutine());
    }

    Coroutine promotionRoutine;

    IEnumerator PromotionRoutine()
    {
        isPromotionChoise = false;
        EnablePromotionUI();
        ChessController.Instance.isTurnEnd = false;
        while (isPromotionChoise == false)
        {
            yield return null;
        }
        Piece promotionInstance = Instantiate(promotionPiece, transform.position, transform.rotation);
        promotionInstance.Init();
        // ���� ���� (Die�� ����)
        Die();
        DisAblePromotionUI();
        ChessController.Instance.isTurnEnd = true;
    }

    void EnablePromotionUI()
    {
        promotionUI = ChessController.Instance.promotionUI;
        promotionUI.UI.SetActive(true);
        promotionUI.UI.transform.position = new Vector3(transform.position.x + 0.5f, 1f, transform.position.z + 0.5f);
        promotionUI.UI.transform.rotation = transform.rotation;

        promotionUI.queen.onClick.AddListener(PromotionQueen);
        promotionUI.rook.onClick.AddListener(PromotionRook);
        promotionUI.bishop.onClick.AddListener(PromotionBishop);
        promotionUI.knight.onClick.AddListener(PromotionKnight);
    }
    void DisAblePromotionUI()
    {
        promotionUI.UI.SetActive(false);

        promotionUI.queen.onClick.RemoveListener(PromotionQueen);
        promotionUI.rook.onClick.RemoveListener(PromotionRook);
        promotionUI.bishop.onClick.RemoveListener(PromotionBishop);
        promotionUI.knight.onClick.RemoveListener(PromotionKnight);
    }

    public void PromotionQueen()
    {
        promotionPiece = team == Team.White ? ChessController.Instance.typeStruct.whiteQueen : ChessController.Instance.typeStruct.blackQueen;
        isPromotionChoise = true;
    }
    public void PromotionRook()
    {
        promotionPiece = team == Team.White ? ChessController.Instance.typeStruct.whiteRook : ChessController.Instance.typeStruct.blackRook;
        isPromotionChoise = true;
    }
    public void PromotionBishop()
    {
        promotionPiece = team == Team.White ? ChessController.Instance.typeStruct.whiteBishop : ChessController.Instance.typeStruct.blackBishop;
        isPromotionChoise = true;
    }
    public void PromotionKnight()
    {
        promotionPiece = team == Team.White ? ChessController.Instance.typeStruct.whiteKnight : ChessController.Instance.typeStruct.blackKnight;
        isPromotionChoise = true;
    }
}
