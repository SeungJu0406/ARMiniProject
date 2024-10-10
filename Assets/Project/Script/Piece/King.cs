using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    Vector3[] dirs = new Vector3[8];

    private void Awake()
    {
        dirs[0] = new Vector3(0, 0, 1);    // ��
        dirs[1] = new Vector3(0, 0, -1);     // ��
        dirs[2] = new Vector3(-1, 0, 0);    // �� 
        dirs[3] = new Vector3(1, 0, 0);   // ��
        dirs[4] = new Vector3(-1, 0, 1);     // �»�
        dirs[5] = new Vector3(1, 0, 1);    // ���
        dirs[6] = new Vector3(-1, 0, -1);   // ����
        dirs[7] = new Vector3(1, 0, -1);    // ����
    }


    public override void AddAbleTile()
    {
        // 1. ���� ��ġ�� ���� ��ġ�� ��ȯ
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position);

        // 2. �ش� ���� ��ġ���� �� �� �ִ� 8���⿡ ���� üũ       
        for (int i = 0; i < dirs.Length; i++)
        {
            BoardPos movePos = curPos;
            movePos.x += (int)dirs[i].x;
            movePos.y += (int)dirs[i].z;

            // 3. �ش� ������ �����ǿ� ���ų� null �� Ÿ���� �ƴϸ� �̵��� �Ұ����ϴ�
            ProcessAbleTile(movePos);
        }
    }
}
