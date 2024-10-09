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
        // ���
        while (true)
        {
            movePos.y += 1;
            // 3. �ش� ��ġ�� ü���� ���̰� null���϶��� ���
            if (!ProcessAbleTile(movePos))
                break;
        }
        // �ϴ�
        movePos = curPos;
        while (true)
        {
            movePos.y -= 1;
            if (!ProcessAbleTile(movePos))
                break;
        }
        // ��
        movePos = curPos;
        while (true)
        {
            movePos.x -= 1;
            if (!ProcessAbleTile(movePos))
                break;
        }
        // ��
        movePos = curPos;
        while (true)
        {
            movePos.x += 1;
            if (!ProcessAbleTile(movePos))
                break;
        }
    }
}
