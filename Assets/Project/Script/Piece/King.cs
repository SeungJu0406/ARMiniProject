using UnityEngine;

public class King : Piece
{
    Vector3[] dirs = new Vector3[8];

    private void Awake()
    {
        dirs[0] = new Vector3(0, 0, 1);    // ��
        dirs[1] = new Vector3(0, 0, -1);     // ��
        dirs[2] = new Vector3(-1, 0, 0);    // �� 
        dirs[3] = new Vector3(1, 0, 0);   // ��
        dirs[4] = new Vector3(-1, 0, 1);     // �»�
        dirs[5] = new Vector3(1, 0, 1);    // ���
        dirs[6] = new Vector3(-1, 0, -1);   // ����
        dirs[7] = new Vector3(1, 0, -1);    // ����
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
        //    Debug.Log($"{piece.team}ŷ üũ!");
        //}
        //else if (ChessBoard.Instance.blackAttackTiles[pos.y, pos.x].warnings.Count > 0)
        //{
        //    Debug.Log($"{piece.team}ŷ üũ ����");
        //}
        //if (ChessBoard.Instance.whiteAttackTiles[pos.y, pos.x].ables.Count > 0)
        //{
        //    Debug.Log($"{piece.team}ŷ üũ!");
        //}
        //else if (ChessBoard.Instance.whiteAttackTiles[pos.y, pos.x].warnings.Count > 0)
        //{
        //    Debug.Log($"{piece.team}ŷ üũ ����");
        //}
    }

    public override void CheckOnWarningTile()
    {
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position); // ���� ��ġ ĳ��

        if (!isCheckWarningAfter)           // �ݺ� ������ ���� ���� bool üũ
        {
            AttackTile[,] attackTiles = team == Team.White ? ChessBoard.Instance.whiteAttackTiles : ChessBoard.Instance.blackAttackTiles;

            if (attackTiles[curPos.y, curPos.x].warnings.Count > 0)
            {
                foreach (Piece warningPiece in attackTiles[curPos.y, curPos.x].warnings)
                {
                    PieceStruct cachingWarningPiece = ChessBoard.GetPieceStruct(warningPiece);
                    cachingWarningPieces.Add(cachingWarningPiece); // ���� �⹰ ĳ��
                }
                ChessBoard.Instance.UnPlacePiece(curPos); // �ӽ÷� ���� ��ġ ����

                for (int i = ablePos.Count - 1; i >= 0; i--)
                {
                    BoardPos movePos = ablePos[i];
                    ChessBoard.Instance.PlacePiece(piece, movePos); // �ӽ÷� �̵��� ��ġ�� ��ġ

                    foreach (PieceStruct warningPiece in cachingWarningPieces)
                    {
                        BoardPos warningPiecePos = ChessBoard.Instance.TransWorldToTile(warningPiece.piece.transform.position);

                        warningPiece.piece.AddAbleTile();

                        attackTiles = team == Team.White ? ChessBoard.Instance.whiteAttackTiles : ChessBoard.Instance.blackAttackTiles;
                        // ���� ����� ŷ�� Ÿ���� able�̵Ǹ�
                        if (attackTiles[movePos.y, movePos.x].ables.Count > 0)
                        {
                            // �ش� Ÿ�� ������ �Ұ�(����Ʈ���� ����)
                            ablePos.RemoveAt(i);
                            ableMoveBoard[movePos.y, movePos.x] = false;
                            break;
                        }
                    }
                    // ables Ŭ����
                    attackTiles[movePos.y, movePos.x].ables.Clear();
                    ChessBoard.Instance.UnPlacePiece(movePos); // �ӽ� �̵� ��ġ ����

                }
                ChessBoard.Instance.PlacePiece(piece, curPos); // ���� ��ġ�� �� ��ġ

                cachingWarningPieces.Clear();
            }
        }

        CreateAbleTile();
    }


    public override void AddAbleTile()
    {
        // 1. ���� ��ġ�� ���� ��ġ�� ��ȯ
        BoardPos curPos = ChessBoard.Instance.TransWorldToTile(transform.position);

        // 2. �ش� ���� ��ġ���� �� �� �ִ� 8���⿡ ���� üũ       
        for (int i = 0; i < dirs.Length; i++)
        {
            BoardPos movePos = curPos;
            movePos.x += (int)dirs[i].x;
            movePos.y += (int)dirs[i].z;

            // 3. �ش� ������ �����ǿ� ���ų� null �� Ÿ���� �ƴϸ� �̵��� �Ұ����ϴ�
            // �߰��� ŷ�� �ش� ĭ�� ableĭ�̸� �ش� ĭ �̵� �Ұ�
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
