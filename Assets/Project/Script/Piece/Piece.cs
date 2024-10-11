using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    [SerializeField] public PieceData data;

    [SerializeField] public Team team;

    public bool isMove;

    protected PieceStruct piece = new PieceStruct();

    public bool[,] ableMoveBoard = new bool[8, 8];
    public List<BoardPos> ablePos = new List<BoardPos>(8);
    public List<BoardPos> warningPos = new List<BoardPos>(8);

    protected List<GameObject> ableTiles = new List<GameObject>(8);

    protected List<PieceStruct> cachingWarningPieces = new List<PieceStruct>();
    protected bool isCheckWarningAfter;

    protected virtual void Start()
    {
        List<Piece> pieces = team == Team.White ? ChessBoard.Instance.whitePieces : ChessBoard.Instance.blackPieces;
        pieces.Add(this);

        BoardPos pos = ChessBoard.Instance.TransWorldToTile(transform.position);
        piece = ChessBoard.GetPieceStruct(this);
        ChessBoard.Instance.PlacePiece(piece, pos);
    }

    public abstract void AddAbleTile();

    public virtual void CheckOnWarningTile()
    {
        if (ablePos.Count == 0) return;

        CheckType kingCheck = team == Team.White ? ChessBoard.Instance.whiteKingCheck : ChessBoard.Instance.blackKingCheck;
        if (kingCheck == CheckType.Check)
        {
            List<Piece> canDefendPiece = team == Team.White ? ChessBoard.Instance.canDefendWhitePiece : ChessBoard.Instance.canDefendBlackPiece;
            // 본인이 방어 불가능 기물이면 모든 ableMoveBoard와 ablePos 삭제
            if (canDefendPiece.Contains(this) == false)
            {
                ClearAbleTile();
            }

        }
        else if (kingCheck == CheckType.Double)
        {
            // 킹만 방어 가능 무조건 ableMoveBoard와 ablePos 삭제
            ClearAbleTile();
        }



        // 워닝 포인트 위의 기물을 이동 할 때
        // 각 이동 포인트에 대해서 워닝포인트를 지정한 기물들에 대해 재연산
        // 이동 결과가 able되면 해당 타일 삭제
        if (!isCheckWarningAfter)           // 반복 연산을 막기 위해 bool 체크
        {
            BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position); // 현재 위치 캐싱
            Piece king = team == Team.White ? ChessBoard.Instance.whiteKing : ChessBoard.Instance.blackKing; // 킹 인스턴스 캐싱
            AttackTile[,] attackTiles = team == Team.White ? ChessBoard.Instance.whiteAttackTiles : ChessBoard.Instance.blackAttackTiles; // 어택 타일 캐싱

            if (attackTiles[curPos.y, curPos.x].warnings.Count > 0)
            {   
                BoardPos kingPos = ChessBoard.Instance.TransWorldToTile(king.transform.position); // 왕의 위치 캐싱            
                foreach (Piece warningPiece in attackTiles[curPos.y, curPos.x].warnings)
                {
                    PieceStruct cachingWarningPiece = ChessBoard.GetPieceStruct(warningPiece);
                    cachingWarningPieces.Add(cachingWarningPiece); // 위험 기물 캐싱
                }
                ChessBoard.Instance.UnPlacePiece(curPos); // 임시로 현재 위치 제거

                for (int i = ablePos.Count - 1; i >= 0; i--)
                {
                    BoardPos movePos = ablePos[i];

                    ChessBoard.Instance.CheckTileOnBoard(movePos, out PieceStruct otherPiece); // 임시로 이동할 위치의 기물 저장
                    ChessBoard.Instance.PlacePiece(piece, movePos); // 임시로 이동한 위치에 배치

                    foreach (PieceStruct warningPiece in cachingWarningPieces)
                    {
                        warningPiece.piece.AddAbleTile();
                        // 연산 결과가 킹의 타일이 able이되면
                        attackTiles = team == Team.White ? ChessBoard.Instance.whiteAttackTiles : ChessBoard.Instance.blackAttackTiles; // 연산 이후 다시 캐싱

                        if (attackTiles[kingPos.y, kingPos.x].ables.Count > 0)
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
                    attackTiles[kingPos.y, kingPos.x].ables.Clear();
                    ChessBoard.Instance.PlacePiece(otherPiece ,movePos); // 임시 위치 다시 복구
                }
                ChessBoard.Instance.PlacePiece(piece, curPos); // 현재 위치로 재 배치

                cachingWarningPieces.Clear();
            }
        }

        isCheckWarningAfter = true;
        // 해당 위치로 able 타일 생성
        CreateAbleTile();
    }

    public void CreateAbleTile()
    {     
        for (int i = 0; i < ablePos.Count; i++)
        {       
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
        ableTiles.Clear();
    }
    public void ClearAbleTile()
    {
        for (int y = 0; y < ableMoveBoard.GetLength(0); y++)
        {
            for (int x = 0; x < ableMoveBoard.GetLength(1); x++)
            {
                ableMoveBoard[y, x] = false;
            }
        }

        ablePos.Clear();
    }
    public bool CheckAbleTile(BoardPos boardPos)
    {
        return ableMoveBoard[boardPos.y, boardPos.x];
    }

    public void Die()
    {
        List<Piece> pieces = team == Team.White ? ChessBoard.Instance.whitePieces : ChessBoard.Instance.blackPieces;
        pieces.Remove(this);
        Destroy(gameObject);
    }

    protected bool ProcessAbleTile(BoardPos movePos)
    {
        isCheckWarningAfter = false;

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

                // 다른 팀이면 AbleZone로 지정 후 리스트에 추가 후 bool 값 false
                ChessBoard.Instance.AddAbleTile(piece, movePos);
                ableMoveBoard[movePos.y, movePos.x] = true;
                ablePos.Add(movePos);
            }
            else if (otherPiece.team != Team.Null && otherPiece.team == team)
            {
                // 같은 팀이면 이동은 불가능하지만 공격은 가능
                ChessBoard.Instance.AddAbleTile(piece, movePos);
            }
        }
        return false;
    }
}
