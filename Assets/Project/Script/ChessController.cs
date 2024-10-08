using System.Collections;
using UnityEngine;

public class ChessController : MonoBehaviour
{
    [Header("기물 잔상 참조")]
    [SerializeField] GameObject pawnPoint;
    [SerializeField] GameObject knightPoint;
    [SerializeField] GameObject bishopPoint;
    [SerializeField] GameObject rookPoint;
    [SerializeField] GameObject queenPoint;
    [SerializeField] GameObject kingPoint;

    GameObject[] piecePoints;
    Piece choicePiece;
    GameObject choicePiecePoint;
    int pieceLayerMask;
    int boardLayerMask;
    private void Awake()
    {
        pieceLayerMask = LayerMask.GetMask("Piece");
        boardLayerMask = LayerMask.GetMask("Board");

        piecePoints = new GameObject[(int)PieceType.Size];
        piecePoints[(int)PieceType.Pawn] = pawnPoint;
        piecePoints[(int)PieceType.Knight] = knightPoint;
        piecePoints[(int)PieceType.Bishop] = bishopPoint;
        piecePoints[(int)PieceType.Rook] = rookPoint;
        piecePoints[(int)PieceType.Queen] = queenPoint;
        piecePoints[(int)PieceType.King] = kingPoint;
        foreach (GameObject point in piecePoints)
        {
            point.gameObject.SetActive(false);
        }
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
            choicePiecePoint = piecePoints[(int)choicePiece.data.type];

            movePieceRoutine = movePieceRoutine == null ? StartCoroutine(MovePieceRoutine()) : movePieceRoutine;
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
            // 기존 위치 제거
            ChessBoard.Instance.UnPlacePiece(ChessBoard.Instance.TransTilePoint(choicePiece.transform.position));
            // 새로운 위치 등록
            choicePiece.transform.position = choicePiecePoint.transform.position;
            ChessBoard.Instance.PlacePiece(choicePiece.data.type, ChessBoard.Instance.TransTilePoint(choicePiece.transform.position));

            ChessBoard.Instance.DebugBoard();

            choicePiecePoint.SetActive(false);
            choicePiece = null;
        }
    }
    Coroutine movePieceRoutine;
    IEnumerator MovePieceRoutine()
    {
        choicePiecePoint.SetActive(true);
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
                choicePiecePoint.transform.position = intPos;
            }
            yield return Manager.Delay.ms05;
        }
    }
}
