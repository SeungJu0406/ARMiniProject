using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    // xoxox
    // oxxxo
    // xxPxx
    // oxxxo
    // xoxox
    
    Vector3[] dirs = new Vector3[8];

    private void Awake()
    {
        dirs[0] = new Vector3(-1,0,2);
        dirs[1] = new Vector3(1,0,2);

        dirs[2] = new Vector3(-2, 0, 1);
        dirs[3] = new Vector3(-2, 0,-1);

        dirs[4] = new Vector3(2,0,1);
        dirs[5] = new Vector3(2,0,-1);

        dirs[6] = new Vector3(-1,0,-2);
        dirs[7] = new Vector3(1, 0,-2);
    }


    public override void CreateAbleTile()
    {
        // 1. ���� ��ġ�� ���� ��ġ�� ��ȯ
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position);

        // 2. �ش� ���� ��ġ���� �� �� �ִ� 8���⿡ ���� üũ       
        for (int i = 0; i < 8; i++) 
        {
            BoardPos movePos = new BoardPos();
            movePos.x = curPos.x + (int)dirs[i].x; 
            movePos.y = curPos.y + (int)dirs[i].z;

            // 3. �ش� ������ �����ǿ� ���ų� null �� Ÿ���� �ƴϸ� �̵��� �Ұ����ϴ�
            if (ChessBoard.Instance.CheckTileInBoard(movePos, out PieceType type))
            {
                // ü���� ���� ������ Ÿ���� null�϶��� ���
                if(type == PieceType.Null)
                {
                    // �ش� ��ġ�� AbleZone�� ���� �� ����Ʈ�� �߰�
                    ableMoveBoard[movePos.y, movePos.x] = true;
                    Vector3 createPos = ChessBoard.Instance.TransTileToWorld(movePos);
                    GameObject ableZone = AbleZonePool.Instance.GetPool(createPos);
                    ableTiles.Add(ableZone);
                }
            }
        }
    }

