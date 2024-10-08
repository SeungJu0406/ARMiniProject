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
    GameObject choicePiece;
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
        piecePoints[(int)PieceType.Bishop]= bishopPoint;
        piecePoints[(int)PieceType.Rook] = rookPoint;
        piecePoints[(int)PieceType.Queen] = queenPoint;
        piecePoints[(int)PieceType.King] = kingPoint;
        foreach (GameObject point in piecePoints)
        {
            Debug.Log(point.name);
            point.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.touchCount == 0) return;
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {         
            ChoosePiece();
        }
        if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            UnChoosePiece();
        }
    }


    void ChoosePiece()
    {
        Vector3 touchPos = Input.touches[0].position;
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 10f, pieceLayerMask))
        {
            choicePiece = hit.collider.gameObject;
            Piece piece = choicePiece.GetComponent<Piece>();
            choicePiecePoint = piecePoints[(int)piece.data.type];
            
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
            choicePiece.transform.position = choicePiecePoint.transform.position;
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
            if (Input.touchCount > 0)
            {
                Vector3 touchPos = Input.touches[0].position;
                Ray ray = Camera.main.ScreenPointToRay(touchPos);
                if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 10f, boardLayerMask))
                {
                    Vector3 intPos = new Vector3((int)hit.point.x, (int)hit.point.y, (int)hit.point.z);
                    choicePiecePoint.transform.position = intPos;
                }
            }
            yield return Manager.Delay.ms05;
        }
    }
}
