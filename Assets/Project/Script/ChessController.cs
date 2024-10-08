using System.Collections;
using UnityEngine;

public class ChessController : MonoBehaviour
{
    [Header("기물 잔상 참조")]
    [SerializeField] GameObject pawnPoint;
    [SerializeField] GameObject knightPoint;
    //[SerializeField] GameObject bishopPoint;
    //[SerializeField] GameObject rookPoint;
    //[SerializeField] GameObject queenPoint;
    //[SerializeField] GameObject KingPoinit;

    GameObject[] piecePoints;
    GameObject choicePiece;
    GameObject choicePiecePoint;
    int pieceLayerMask;
    int boardLayerMask;
    private void Awake()
    {
        pieceLayerMask = LayerMask.GetMask("Piece");
        boardLayerMask = LayerMask.GetMask("Board");

        piecePoints = new GameObject[2];
        piecePoints[(int)PieceType.Pawn] = pawnPoint;
        piecePoints[(int)PieceType.Knight] = knightPoint;

        foreach(GameObject point in piecePoints)
        {
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
        Debug.Log("터치");
        Vector3 touchPos = Input.touches[0].position;
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 10f, pieceLayerMask))
        {
            Debug.Log("선택");
            choicePiece = hit.collider.gameObject;
            Piece piece = choicePiece.GetComponent<Piece>();
            choicePiecePoint = piecePoints[(int)piece.data.type];
            
            movePieceRoutine = movePieceRoutine == null ? StartCoroutine(MovePieceRoutine()) : movePieceRoutine;
        }

    }
    void UnChoosePiece()
    {
        Debug.Log("터치해제");
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
                Debug.Log("이동");
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
