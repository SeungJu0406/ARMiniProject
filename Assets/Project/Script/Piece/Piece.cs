using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    [SerializeField] public PieceData data;

    [SerializeField] public Team team;

    public bool isMove;

    protected PieceStruct piece = new PieceStruct();

    protected bool[,] ableMoveBoard = new bool[8, 8];
    protected List<GameObject> ableTiles = new List<GameObject>(8);
    protected List<BoardPos> ablePos = new List<BoardPos>(8);
    protected List<BoardPos> warningPos = new List<BoardPos>(8);

    List<PieceStruct> cachingWarningPieces = new List<PieceStruct>();

    protected virtual void Start()
    {
        ChessBoard.Instance.pieces.Add(this);

        BoardPos pos = ChessBoard.Instance.TransWorldToTile(transform.position);
        piece = ChessBoard.GetPieceStruct(this);
        ChessBoard.Instance.PlacePiece(piece, pos);
    }

    public abstract void AddAbleTile();

    public virtual void CheckOnWarningTile()
    {
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position); // 현재 위치 캐싱
        // 워닝 포인트 위의 기물을 이동 할 때
        // 각 이동 포인트에 대해서 워닝포인트를 지정한 기물들에 대해 재연산
        // 이동 결과가 able되면 해당 타일 삭제
        if (team == Team.White && ChessBoard.Instance.blackAttackTiles[curPos.y, curPos.x].warnings.Count > 0)
        {
            BoardPos kingPos = ChessBoard.Instance.TransWorldToTile(ChessBoard.Instance.whiteKing.transform.position); // 왕의 위치 캐싱
            foreach (Piece warningPiece in ChessBoard.Instance.blackAttackTiles[curPos.y, curPos.x].warnings)
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
                    warningPiece.piece.AddAbleTile();
                    // 연산 결과가 킹의 타일이 able이되면
                    if (ChessBoard.Instance.blackAttackTiles[kingPos.y, kingPos.x].ables.Count > 0)
                    {
                        // 해당 적의 위치와 movePos 가 같은 경우는 제외 (잡은 경우)
                        BoardPos warningPiecePos = ChessBoard.Instance.TransWorldToTile(warningPiece.piece.transform.position);
                        if (warningPiecePos.x == movePos.x && warningPiecePos.y == movePos.y)
                            continue;

                        // 해당 타일 움직임 불가(리스트에서 제거)
                        ablePos.RemoveAt(i);
                        ableMoveBoard[movePos.y, movePos.x] = false;
                        break;
                    }
                }
                // ables 클리어
                ChessBoard.Instance.blackAttackTiles[kingPos.y, kingPos.x].ables.Clear();
                ChessBoard.Instance.UnPlacePiece(movePos); // 임시 이동 위치 삭제
            }
            ChessBoard.Instance.PlacePiece(piece, curPos); // 현재 위치로 재 배치

            cachingWarningPieces.Clear();
        }
        else if (team == Team.Black && ChessBoard.Instance.whiteAttackTiles[curPos.y, curPos.x].warnings.Count > 0)
        {
            BoardPos kingPos = ChessBoard.Instance.TransWorldToTile(ChessBoard.Instance.blackKing.transform.position); // 왕의 위치 캐싱
            foreach (Piece warningPiece in ChessBoard.Instance.whiteAttackTiles[curPos.y, curPos.x].warnings)
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
                    warningPiece.piece.AddAbleTile();
                    // 연산 결과가 킹의 타일이 able이되면
                    if (ChessBoard.Instance.whiteAttackTiles[kingPos.y, kingPos.x].ables.Count > 0)
                    {
                        // 해당 적의 위치와 movePos 가 같은 경우는 제외 (잡은 경우)
                        BoardPos warningPiecePos = ChessBoard.Instance.TransWorldToTile(warningPiece.piece.transform.position);
                        if (warningPiecePos.x == movePos.x && warningPiecePos.y == movePos.y)
                            continue;

                        // 해당 타일 움직임 불가(리스트에서 제거)
                        ablePos.RemoveAt(i);
                        ableMoveBoard[movePos.y, movePos.x] = false;
                        break;
                    }
                }
                // ables 클리어
                ChessBoard.Instance.whiteAttackTiles[kingPos.y, kingPos.x].ables.Clear();
                ChessBoard.Instance.UnPlacePiece(movePos); // 임시 이동 위치 삭제
            }
            ChessBoard.Instance.PlacePiece(piece, curPos); // 현재 위치로 재 배치

            cachingWarningPieces.Clear();
        }

        // 해당 위치로 able 타일 생성
        CreateAbleTile();
    }

    public void CreateAbleTile()
    {
        Debug.Log($"타일 개수{ablePos.Count}");
        for (int i = 0; i < ablePos.Count; i++)
        {
            Debug.Log($"번호 {i}");
            Vector3 pos = ChessBoard.Instance.TransTileToWorld(ablePos[i]);
            GameObject ableTile = AbleZonePool.Instance.GetPool(pos);
            ableTiles.Add(ableTile);
        }
    }
    public void RemoveAbleTile()
    {
        foreach (GameObject tile in ableTiles)
        {
            AbleZonePool.Instance.ReturnPool(tile);
        }
    }
    public void ClearAbleTiel()
    {
        for (int y = 0; y < ableMoveBoard.GetLength(0); y++)
        {
            for (int x = 0; x < ableMoveBoard.GetLength(1); x++)
            {
                ableMoveBoard[y, x] = false;
            }
        }
        ableTiles.Clear();
        ablePos.Clear();
    }
    public bool CheckAbleTile(BoardPos boardPos)
    {
        return ableMoveBoard[boardPos.y, boardPos.x];
    }

    public void Die()
    {
        ChessBoard.Instance.pieces.Remove(this);
        Destroy(gameObject);
    }

    protected bool ProcessAbleTile(BoardPos movePos)
    {
        if (ChessBoard.Instance.CheckTileOnBoard(movePos, out PieceStruct otherPiece))
        {
            if (otherPiece.type == PieceType.Null)
            {
                // 해당 위치를 AbleZone로 지정 후 리스트에 추가
                ChessBoard.Instance.AddAbleTile(piece, movePos);
                ableMoveBoard[movePos.y, movePos.x] = true;
                ablePos.Add(movePos);
                return true;
            }
            else if (otherPiece.team != Team.Null && otherPiece.team != team)
            {
                if (movePos.y == 7 && movePos.x == 4)
                    Debug.Log(otherPiece.type);

                // 다른 팀이면 AbleZone로 지정 후 리스트에 추가 후 bool 값 false
                ChessBoard.Instance.AddAbleTile(piece, movePos);
                ableMoveBoard[movePos.y, movePos.x] = true;
                ablePos.Add(movePos);
            }
        }
        return false;
    }
}
