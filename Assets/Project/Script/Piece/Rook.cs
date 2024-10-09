using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{ 

    public override void CreateAbleTile()
    {
        // 1. 현재 위치를 보드 위치로 변환
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position);
        // 2. 갈 수 있는 4방향에 대해 체크    
        BoardPos movePos = curPos;
        // 상단
        while (true)
        {
            movePos.y += 1;
            // 3. 해당 위치가 체스판 위이고 null값일때만 통과
            if (!ProcessAbleTile(movePos))
                break;
        }
        // 하단
        movePos = curPos;
        while (true)
        {
            movePos.y -= 1;
            if (!ProcessAbleTile(movePos))
                break;
        }
        // 좌
        movePos = curPos;
        while (true)
        {
            movePos.x -= 1;
            if (!ProcessAbleTile(movePos))
                break;
        }
        // 우
        movePos = curPos;
        while (true)
        {
            movePos.x += 1;
            if (!ProcessAbleTile(movePos))
                break;
        }
    }
}
