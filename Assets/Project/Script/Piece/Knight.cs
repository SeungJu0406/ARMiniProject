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
        dirs[0] = new Vector3(-1, 0, 2);
        dirs[1] = new Vector3(1, 0, 2);

        dirs[2] = new Vector3(-2, 0, 1);
        dirs[3] = new Vector3(-2, 0, -1);

        dirs[4] = new Vector3(2, 0, 1);
        dirs[5] = new Vector3(2, 0, -1);

        dirs[6] = new Vector3(-1, 0, -2);
        dirs[7] = new Vector3(1, 0, -2);
    }


    public override void AddAbleTile()
    {
        // 1. 현재 위치를 보드 위치로 변환
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position);

        warningPos.Add(curPos);
        ChessBoard.Instance.AddWarningTile(piece, curPos); // 나이트는 본인의 위치만 워닝포인트로 지정

        // 2. 해당 보드 위치에서 갈 수 있는 8방향에 대해 체크       
        for (int i = 0; i < dirs.Length; i++)
        {
            BoardPos movePos = curPos;
            movePos.x += (int)dirs[i].x;
            movePos.y += (int)dirs[i].z;

            // 3. 해당 방향이 보드판에 없거나 null 값 타일이 아니면 이동이 불가능하다
            ProcessAbleTile(movePos);
        }
    }
}

