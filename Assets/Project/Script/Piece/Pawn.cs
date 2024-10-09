using UnityEngine;

public class Pawn : Piece
{
    Vector3[] dirs = new Vector3[3];
    public override void CreateAbleTile()
    {
        // 1. ���� ��ġ�� ���� ��ġ�� ��ȯ
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position);
        // 2. �� �� �ִ� ���⿡ ���� üũ  
        // 2-1. ���� ��鿡 ���� ������ �ٸ���
        if (team == Team.Black)
        {
            dirs[0] = new Vector3(0, 0, 1);
            dirs[1] = new Vector3(-1, 0, 1);
            dirs[2] = new Vector3(1, 0, 1);
        }
        else if(team == Team.White)
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
                    ChessBoard.Instance.CheckTileOnBoard(movePos, out PieceStruct piece);
                    if (piece.type == PieceType.Null)
                    {
                        if (!ProcessAbleTile(movePos))
                            continue;
                    }
                }
            }
            // �밢�� �̵��� ��� �ش� ��ġ�� ���� ���� ���� ������ �� �ִ�
            else
            {
                movePos.x += (int)dirs[i].x;
                movePos.y += (int)dirs[i].z;

                ChessBoard.Instance.CheckTileOnBoard(movePos, out PieceStruct piece);
                if (piece.team != Team.Null && piece.team != team)
                {
                    if (!ProcessAbleTile(movePos))
                        continue;
                }
            }
        }
    }
}
