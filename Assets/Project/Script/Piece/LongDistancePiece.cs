public abstract class LongDistancePiece : Piece
{
    public override void AddAbleTile() { }

    protected bool CheckAttackKing(BoardPos boardPos,out PieceStruct otherPiece)
    {
        if (ChessBoard.Instance.CheckTileOnBoard(boardPos, out otherPiece))
        {
            if (otherPiece.team != team && otherPiece.type == PieceType.King) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
        
    }
}
