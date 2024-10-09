using UnityEngine;

public class Pawn : Piece
{
    Vector3[] dirs = new Vector3[3];
    public override void CreateAbleTile()
    {
        // 1. 현재 위치를 보드 위치로 변환
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position);
        // 2. 갈 수 있는 방향에 대해 체크  
        // 2-1. 폰은 흑백에 따라 방향이 다르다
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
            // 전방 이동의 경우       
            if (i == 0)
            {
                // 움직인 적이 없었다면 2칸까지 움직일 수 있다   
                int count;
                if (isMove)
                    count = 1;
                else
                    count = 2;
                for (int j = 0; j < count; j++)
                {
                    movePos.y += (int)dirs[i].z;
                    // 3. 해당 위치가 체스판 위이고 null값일때만 통과
                    // 앞에 기물이 없을 때만 움직일 수 있다
                    ChessBoard.Instance.CheckTileOnBoard(movePos, out PieceStruct piece);
                    if (piece.type == PieceType.Null)
                    {
                        if (!ProcessAbleTile(movePos))
                            continue;
                    }
                }
            }
            // 대각선 이동의 경우 해당 위치에 적이 있을 때만 움직일 수 있다
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
