using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    public override void CreateAbleTile()
    {
        // 1. 현재 위치를 보드 위치로 변환
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position);
        // 2. 갈 수 있는 4방향에 대해 체크    
        BoardPos movePos = curPos;
        // 좌상
        while (true)
        {
            movePos.x -= 1;
            movePos.y += 1;
            // 3. 해당 위치가 체스판 위이고 null값일때만 통과
            if (!ProcessAbleTile(movePos))
                break;
        }
        // 우상
        movePos = curPos;
        while (true)
        {
            movePos.x += 1;
            movePos.y += 1;
            if (!ProcessAbleTile(movePos))
                break;
        }
        // 좌하
        movePos = curPos;
        while (true)
        {
            movePos.x -= 1;
            movePos.y -= 1;
            if (!ProcessAbleTile(movePos))
                break;
        }
        // 우하
        movePos = curPos;
        while (true)
        {
            movePos.x += 1;
            movePos.y -= 1;
            if (!ProcessAbleTile(movePos))
                break;
        }
    }

}
