using System.Collections;
using UnityEngine;

public class ChessController : MonoBehaviour
{
    [SerializeField] GameObject pawnPosPrefab;
    GameObject pawnPosInstance;
    GameObject choicePiece;
    int pieceLayerMask;
    int boardLayerMask;
    private void Awake()
    {
        pieceLayerMask = LayerMask.GetMask("Piece");
        boardLayerMask = LayerMask.GetMask("Board");

        pawnPosInstance = Instantiate(pawnPosPrefab);
        pawnPosInstance.SetActive(false);
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
            choicePiece.transform.position = pawnPosInstance.transform.position;
            pawnPosInstance.SetActive(false);
            choicePiece = null;
        }

    }
    Coroutine movePieceRoutine;
    IEnumerator MovePieceRoutine()
    {
        pawnPosInstance.SetActive(true);
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
                    pawnPosInstance.transform.position = intPos;
                }
            }
            yield return Manager.Delay.ms05;
        }
    }
}
