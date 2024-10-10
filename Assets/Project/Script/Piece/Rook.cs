using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{ 

    public override void CreateAbleTile()
    {
        // 1. ���� ��ġ�� ���� ��ġ�� ��ȯ
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position);
        // 2. �� �� �ִ� 4���⿡ ���� üũ    
        BoardPos movePos = curPos;

        for(int i= 0; i< 4; i++)
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
                }
                if (!ProcessAbleTile(movePos))
                    break;
            }        
        }
    }
}
