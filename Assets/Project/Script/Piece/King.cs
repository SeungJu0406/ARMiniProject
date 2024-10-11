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
        if (team == Team.Black)
        {
            ChessBoard.Instance.blackKing = this;
        }
        else if (team == Team.White)
        {
            ChessBoard.Instance.whiteKing = this;
        }


        BoardPos pos = ChessBoard.Instance.TransWorldToTile(transform.position);
        piece = ChessBoard.GetPieceStruct(this);
        ChessBoard.Instance.PlacePiece(piece, pos);
    }


    private void Update()
    {
        //BoardPos pos = ChessBoard.Instance.TransWorldToTile(transform.position);
        //if (ChessBoard.Instance.blackAttackTiles[pos.y, pos.x].ables.Count > 0)
        //{
        //    Debug.Log($"{piece.team}킹 체크!");
        //}
        //else if (ChessBoard.Instance.blackAttackTiles[pos.y, pos.x].warnings.Count > 0)
        //{
        //    Debug.Log($"{piece.team}킹 체크 위험");
        //}
        //if (ChessBoard.Instance.whiteAttackTiles[pos.y, pos.x].ables.Count > 0)
        //{
        //    Debug.Log($"{piece.team}킹 체크!");
        //}
        //else if (ChessBoard.Instance.whiteAttackTiles[pos.y, pos.x].warnings.Count > 0)
        //{
        //    Debug.Log($"{piece.team}킹 체크 위험");
        //}
    }

    public override void CheckOnWarningTile()
    {
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position); // 현재 위치 캐싱

        if (!isCheckWarningAfter)           // 반복 연산을 막기 위해 bool 체크
        {
            AttackTile[,] attackTiles = team == Team.White ? ChessBoard.Instance.whiteAttackTiles : ChessBoard.Instance.blackAttackTiles;

            if (attackTiles[curPos.y, curPos.x].warnings.Count > 0)
            {
                foreach (Piece warningPiece in attackTiles[curPos.y, curPos.x].warnings)
                {
                    PieceStruct cachingWarningPiece = ChessBoard.GetPieceStruct(warningPiece);
                    cachingWarningPieces.Add(cachingWarningPiece); // 위험 기물 캐싱
                }
                ChessBoard.Instance.UnPlacePiece(curPos); // 임시로 현재 위치 제거

                for (int i = ablePos.Count - 1; i >= 0; i--)
                {
                    BoardPos movePos = ablePos[i];
                    ChessBoard.Instance.PlacePiece(piece, movePos); // 임시로 이동한 위치에 배치

                    foreach (PieceStruct warningPiece in cachingWarningPieces)
                    {
                        BoardPos warningPiecePos = ChessBoard.Instance.TransWorldToTile(warningPiece.piece.transform.position);

                        warningPiece.piece.AddAbleTile();

                        attackTiles = team == Team.White ? ChessBoard.Instance.whiteAttackTiles : ChessBoard.Instance.blackAttackTiles;
                        // 연산 결과가 킹의 타일이 able이되면
                        if (attackTiles[movePos.y, movePos.x].ables.Count > 0)
                        {
                            // 해당 타일 움직임 불가(리스트에서 제거)
                            ablePos.RemoveAt(i);
                            ableMoveBoard[movePos.y, movePos.x] = false;
                            break;
                        }
                    }
                    // ables 클리어
                    attackTiles[movePos.y, movePos.x].ables.Clear();
                    ChessBoard.Instance.UnPlacePiece(movePos); // 임시 이동 위치 삭제

                }
                ChessBoard.Instance.PlacePiece(piece, curPos); // 현재 위치로 재 배치

                cachingWarningPieces.Clear();
            }
        }

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
            // 추가로 킹은 해당 칸이 able칸이면 해당 칸 이동 불가
            AttackTile[,] attackTiles = team == Team.Black ? ChessBoard.Instance.blackAttackTiles : ChessBoard.Instance.whiteAttackTiles;

            if (ChessBoard.Instance.CheckTileOnBoard(movePos, out PieceStruct otherPiece))
            {
                if (attackTiles[movePos.y, movePos.x].ables.Count == 0)
                {
                    ProcessAbleTile(movePos);
                }
            }
        }
    }
}
