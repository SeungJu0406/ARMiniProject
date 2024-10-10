using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    Vector3[] dirs = new Vector3[8];

    private void Awake()
    {
        dirs[0] = new Vector3(0, 0, 1);    // 상
        dirs[1] = new Vector3(0, 0, -1);     // 하
        dirs[2] = new Vector3(-1, 0, 0);    // 좌 
        dirs[3] = new Vector3(1, 0, 0);   // 우
        dirs[4] = new Vector3(-1, 0, 1);     // 좌상
        dirs[5] = new Vector3(1, 0, 1);    // 우상
        dirs[6] = new Vector3(-1, 0, -1);   // 좌하
        dirs[7] = new Vector3(1, 0, -1);    // 우하
    }

    protected override void Start()
    {
        base.Start();

        if (team == Team.Black)
        {
            ChessBoard.Instance.blackKing = this;
        }
        else if (team == Team.White) 
        {
            ChessBoard.Instance.whiteKing = this;
        }
    }


    private void Update()
    {
        //BoardPos pos = ChessBoard.Instance.TransWorldToTile(transform.position);
        //if (ChessBoard.Instance.whiteAttackTiles[pos.y, pos.x].ables.Count > 0)
        //{
        //    Debug.Log($"{piece.team}킹 체크!");
        //}
        //else if(ChessBoard.Instance.whiteAttackTiles[pos.y, pos.x].warnings.Count > 0)
        //{
        //    Debug.Log($"{piece.team}킹 체크 위험");
        //}
        //if (ChessBoard.Instance.blackAttackTiles[pos.y, pos.x].ables.Count > 0)
        //{
        //    Debug.Log($"{piece.team}킹 체크!");
        //}
        //else if (ChessBoard.Instance.blackAttackTiles[pos.y, pos.x].warnings.Count > 0)
        //{
        //    Debug.Log($"{piece.team}킹 체크 위험");
        //}
    }

    public override void CheckOnWarningTile()
    {
        CreateAbleTile();
    }


    public override void AddAbleTile()
    {
        // 1. 현재 위치를 보드 위치로 변환
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position);

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
