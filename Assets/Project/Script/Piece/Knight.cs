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
        // 1. 현재 위치를 보드 위치로 변환
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position);

        // 2. 해당 보드 위치에서 갈 수 있는 8방향에 대해 체크       
        for (int i = 0; i < 8; i++) 
        {
            BoardPos movePos = new BoardPos();
            movePos.x = curPos.x + (int)dirs[i].x; 
            movePos.y = curPos.y + (int)dirs[i].z;

            // 3. 해당 방향이 보드판에 없거나 null 값 타일이 아니면 이동이 불가능하다
            if (ChessBoard.Instance.CheckTileInBoard(movePos, out PieceType type))
            {
                // 체스판 위에 있지만 타일이 null일때만 통과
                if(type == PieceType.Null)
                {
                    // 해당 위치를 AbleZone로 지정 후 리스트에 추가
                    ableMoveBoard[movePos.y, movePos.x] = true;
                    Vector3 createPos = ChessBoard.Instance.TransTileToWorld(movePos);
                    GameObject ableZone = AbleZonePool.Instance.GetPool(createPos);
                    ableTiles.Add(ableZone);
                }
            }
        }
    }

