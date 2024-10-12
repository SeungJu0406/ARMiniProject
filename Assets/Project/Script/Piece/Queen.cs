using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Queen : LongDistancePiece
{

    public override void AddAbleTile()
    {
        // 1. 현재 위치를 보드 위치로 변환
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position);
        // 2. 갈 수 있는 8방향에 대해 체크    
        BoardPos movePos = curPos;
        PieceStruct otherPiece = new PieceStruct();
        for (int i = 0; i < 8; i++)
        {
            movePos = curPos;
            while (true)
            {
                switch (i)
                {
                    case 0:
                        movePos.y += 1;
                        break;
                    case 1:
                        movePos.y -= 1;
                        break;
                    case 2:
                        movePos.x -= 1;
                        break;
                    case 3:
                        movePos.x += 1;
                        break;
                    case 4:
                        movePos.x -= 1;
                        movePos.y += 1;
                        break;
                    case 5:
                        movePos.x += 1;
                        movePos.y += 1;
                        break;
                    case 6:
                        movePos.x -= 1;
                        movePos.y -= 1;
                        break;
                    case 7:
                        movePos.x += 1;
                        movePos.y -= 1;
                        break;
                }
                if (CheckAttackKing(movePos, out otherPiece))
                {
                    break;
                }                                       
            }
            if (otherPiece.type == PieceType.King)
            {
                while (true)
                {
                    warningPos.Add(movePos);
                    ChessBoard.Instance.AddWarningTile(piece, movePos);
                    if (movePos.x == curPos.x && movePos.y == curPos.y)
                        break;

                    switch (i)
                    {
                        case 0:
                            movePos.y += -1;
                            break;
                        case 1:
                            movePos.y -= -1;
                            break;
                        case 2:
                            movePos.x -= -1;
                            break;
                        case 3:
                            movePos.x += -1;
                            break;
                        case 4:
                            movePos.x -= -1;
                            movePos.y += -1;
                            break;
                        case 5:
                            movePos.x += -1;
                            movePos.y += -1;
                            break;
                        case 6:
                            movePos.x -= -1;
                            movePos.y -= -1;
                            break;
                        case 7:
                            movePos.x += -1;
                            movePos.y -= -1;
                            break;
                    }
                }
            }
        }

        for (int i = 0; i < 8; i++)
        {
            movePos = curPos;
            while (true)
            {
                switch (i)
                {
                    case 0:
                        movePos.y += 1;
                        break;
                    case 1:
                        movePos.y -= 1;
                        break;
                    case 2:
                        movePos.x -= 1;
                        break;
                    case 3:
                        movePos.x += 1;
                        break;
                    case 4:
                        movePos.x -= 1;
                        movePos.y += 1;
                        break;
                    case 5:
                        movePos.x += 1;
                        movePos.y += 1;
                        break;
                    case 6:
                        movePos.x -= 1;
                        movePos.y -= 1;
                        break;
                    case 7:
                        movePos.x += 1;
                        movePos.y -= 1;
                        break;
                }
                if (!ProcessAbleTile(movePos))
                    break;
            }
        }
    }
}
