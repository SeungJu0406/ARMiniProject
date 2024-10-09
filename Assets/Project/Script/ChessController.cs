using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ChessController : MonoBehaviour
{
    [System.Serializable]
    public struct Point
    {
        [SerializeField]public GameObject pawnPoint;
        [SerializeField]public GameObject knightPoint;
        [SerializeField]public GameObject bishopPoint;
        [SerializeField]public GameObject rookPoint;
        [SerializeField]public GameObject queenPoint;
        [SerializeField]public GameObject kingPoint;
    }
    [SerializeField] Point point;

    [System.Serializable]
    public struct PiecePoint
    {
       public GameObject piecePoint;
       public Material pointMaterial;
    }
    PiecePoint[] piecePoints;

    Piece choicePiece;
    PiecePoint choicePiecePoint;

    Team curTeam = Team.Black;

    int pieceLayerMask;
    int boardLayerMask;
    private void Awake()
    {
        pieceLayerMask = LayerMask.GetMask("Piece");
        boardLayerMask = LayerMask.GetMask("Board");

        InitPoint();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
#else
        if (Input.touchCount == 0) return;
        if (Input.GetTouch(0).phase == TouchPhase.Began)
#endif
        {
            ChoosePiece();
        }
#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0))
#else
        if (Input.GetTouch(0).phase == TouchPhase.Ended)
#endif
        {
            UnChoosePiece();
        }
    }


    void ChoosePiece()
    {
#if UNITY_EDITOR
        Vector3 touchPos = Input.mousePosition;
#else
        Vector3 touchPos = Input.touches[0].position;
#endif
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 10f, pieceLayerMask))
        {
            choicePiece = hit.collider.GetComponent<Piece>();
            if (choicePiece.team == curTeam)
            {
                choicePiecePoint = piecePoints[(int)choicePiece.data.type];
                choicePiece.CreateAbleTile();

                movePieceRoutine = movePieceRoutine == null ? StartCoroutine(MovePieceRoutine()) : movePieceRoutine;
            }
            else
            {
                choicePiece = null;
            }
        }

    }
    void UnChoosePiece()
    {
        if (movePieceRoutine != null)
        {
            StopCoroutine(movePieceRoutine);
            movePieceRoutine = null;
        }

        if (choicePiece != null)
        {
            BoardPos pointPos = ChessBoard.Instance.TransWorldToTile(choicePiecePoint.piecePoint.transform.position);
            if (choicePiece.CheckAbleTile(pointPos))
            {
                // 해당 위치에 상대 기물이 있다면 상대 기물을 제거
                if (ChessBoard.Instance.CheckTileOnBoard(pointPos, out PieceStruct enemyPiece))
                {
                    if(enemyPiece.team != Team.Null && enemyPiece.team != choicePiece.team)
                    {
                        Destroy(enemyPiece.piece.gameObject);
                    }
                }

                // 기존 위치 제거
                ChessBoard.Instance.UnPlacePiece(ChessBoard.Instance.TransWorldToTile(choicePiece.transform.position));
                // 새로운 위치 등록
                choicePiece.transform.position = choicePiecePoint.piecePoint.transform.position;
                PieceStruct piece = ChessBoard.GetPieceStruct(choicePiece, choicePiece.team);
                ChessBoard.Instance.PlacePiece(piece, ChessBoard.Instance.TransWorldToTile(choicePiece.transform.position));
                choicePiece.isMove = true;

                // 턴 넘기기
                if(choicePiece.team == Team.Black)
                    curTeam = Team.White;
                else if (choicePiece.team == Team.White)
                    curTeam = Team.Black;
            }

            ChessBoard.Instance.DebugBoard();
            choicePiece.RemoveAbleTile();
            choicePiecePoint.piecePoint.SetActive(false);
            choicePiece = null;
        }
    }
    Coroutine movePieceRoutine;
    IEnumerator MovePieceRoutine()
    {
        choicePiecePoint.piecePoint.SetActive(true);
        while (true)
        {
#if UNITY_EDITOR
            Vector3 touchPos = Input.mousePosition;
#else
            Vector3 touchPos = Input.touches[0].position;
#endif
            Ray ray = Camera.main.ScreenPointToRay(touchPos);
            Debug.DrawRay(ray.origin, ray.direction * 100f);
            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 10f, boardLayerMask))
            {
                Vector3 intPos = new Vector3((int)hit.point.x, (int)hit.point.y, (int)hit.point.z);
                BoardPos boardPos = ChessBoard.Instance.TransWorldToTile(intPos);
                if (choicePiece.CheckAbleTile(boardPos)) 
                {
                    Color color = Color.cyan;
                    color.a = 0.3f;
                    choicePiecePoint.pointMaterial.color = color;
                }
                else
                {
                    Color color = Color.red;
                    color.a = 0.3f;
                    choicePiecePoint.pointMaterial.color = color;
                }
                choicePiecePoint.piecePoint.transform.position = intPos;
                choicePiecePoint.piecePoint.transform.rotation = choicePiece.transform.rotation;
            }
            yield return Manager.Delay.ms05;
        }
    }


    private void InitPoint()
    {
        piecePoints = new PiecePoint[(int)PieceType.Size];
        piecePoints[(int)PieceType.Pawn] = new PiecePoint();
        piecePoints[(int)PieceType.Pawn].piecePoint = point.pawnPoint;
        piecePoints[(int)PieceType.Pawn].pointMaterial = piecePoints[(int)PieceType.Pawn].piecePoint.GetComponentInChildren<MeshRenderer>().material;

        piecePoints[(int)PieceType.Knight] = new PiecePoint();
        piecePoints[(int)PieceType.Knight].piecePoint = point.knightPoint;
        piecePoints[(int)PieceType.Knight].pointMaterial = piecePoints[(int)PieceType.Knight].piecePoint.GetComponentInChildren<MeshRenderer>().material;

        piecePoints[(int)PieceType.Bishop] = new PiecePoint();
        piecePoints[(int)PieceType.Bishop].piecePoint = point.bishopPoint;
        piecePoints[(int)PieceType.Bishop].pointMaterial = piecePoints[(int)PieceType.Bishop].piecePoint.GetComponentInChildren<MeshRenderer>().material;

        piecePoints[(int)PieceType.Rook] = new PiecePoint();
        piecePoints[(int)PieceType.Rook].piecePoint = point.rookPoint;
        piecePoints[(int)PieceType.Rook].pointMaterial = piecePoints[(int)PieceType.Rook].piecePoint.GetComponentInChildren<MeshRenderer>().material;

        piecePoints[(int)PieceType.Queen] = new PiecePoint();
        piecePoints[(int)PieceType.Queen].piecePoint = point.queenPoint;
        piecePoints[(int)PieceType.Queen].pointMaterial = piecePoints[(int)PieceType.Queen].piecePoint.GetComponentInChildren<MeshRenderer>().material;

        piecePoints[(int)PieceType.King] = new PiecePoint();
        piecePoints[(int)PieceType.King].piecePoint = point.kingPoint;
        piecePoints[(int)PieceType.King].pointMaterial = piecePoints[(int)PieceType.King].piecePoint.GetComponentInChildren<MeshRenderer>().material;

        foreach (PiecePoint point in piecePoints)
        {
            point.piecePoint.SetActive(false);
        }
    }
}
