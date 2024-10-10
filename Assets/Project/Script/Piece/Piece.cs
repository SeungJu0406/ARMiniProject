using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    [SerializeField] public PieceData data;

    [SerializeField] public Team team;

    public bool isMove;

    protected bool[,] ableMoveBoard = new bool[8, 8];
    protected List<GameObject> ableTiles = new List<GameObject>(8);

    

    private void Start()
    {
        ChessBoard.Instance.pieces.Add(this);

        BoardPos pos = ChessBoard.Instance.TransWorldToTile(transform.position);
        PieceStruct piece = ChessBoard.GetPieceStruct(this, team);
        ChessBoard.Instance.PlacePiece(piece, pos);
       
    }

    public abstract void CreateAbleTile();
    public void RemoveAbleTile()
    {
        for (int y = 0; y < ableMoveBoard.GetLength(0); y++)
        {
            for (int x = 0; x < ableMoveBoard.GetLength(1); x++)
            {
                ableMoveBoard[y, x] = false;
            }
        }

        foreach (GameObject tile in ableTiles)
        {
            AbleZonePool.Instance.ReturnPool(tile);
        }
        ableTiles.Clear();
    }
    public bool CheckAbleTile(BoardPos boardPos)
    {
        return ableMoveBoard[boardPos.y, boardPos.x];
    }

    protected bool ProcessAbleTile(BoardPos movePos)
    {
        if (ChessBoard.Instance.CheckTileOnBoard(movePos, out PieceStruct piece))
        {
            if (piece.type == PieceType.Null)
            {
                // 해당 위치를 AbleZone로 지정 후 리스트에 추가
                ableMoveBoard[movePos.y, movePos.x] = true;
                Vector3 createPos = ChessBoard.Instance.TransTileToWorld(movePos);
                GameObject ableTile = AbleZonePool.Instance.GetPool(createPos);
                ableTiles.Add(ableTile);
                return true;
            }
            else if(piece.team != Team.Null && piece.team != team)
            {
                // 다른 팀이면 AbleZone로 지정 후 리스트에 추가 후 bool 값 false
                ableMoveBoard[movePos.y, movePos.x] = true;
                Vector3 createPos = ChessBoard.Instance.TransTileToWorld(movePos);
                GameObject ableTile = AbleZonePool.Instance.GetPool(createPos);
                ableTiles.Add(ableTile);
            }
        }
        return false;
    }
}
