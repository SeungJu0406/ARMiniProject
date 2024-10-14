using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct TypeStruct
{
    [Header("White")]
    public Pawn whitePawn;
    public Knight whiteKnight;
    public Bishop whiteBishop;
    public Rook whiteRook;
    public Queen whiteQueen;
    public King whiteKing;
    [Header("Black")]
    public Pawn blackPawn;
    public Knight blackKnight;
    public Bishop blackBishop;
    public Rook blackRook;
    public Queen blackQueen;
    public King blackKing;
}
[System.Serializable]
public struct PromotionUI
{
    public GameObject UI;
    public Button queen;
    public Button rook;
    public Button bishop;
    public Button knight;
}

public class ChessController : MonoBehaviour
{
    public static ChessController Instance;


    public TypeStruct typeStruct;

    [System.Serializable]
    public struct Point
    {
        [SerializeField] public GameObject pawnPoint;
        [SerializeField] public GameObject knightPoint;
        [SerializeField] public GameObject bishopPoint;
        [SerializeField] public GameObject rookPoint;
        [SerializeField] public GameObject queenPoint;
        [SerializeField] public GameObject kingPoint;
    }
    [SerializeField] Point point;

    [System.Serializable]
    public struct PiecePoint
    {
        public GameObject piecePoint;
        public Material pointMaterial;
    }

    [SerializeField] public PromotionUI promotionUI;

    PiecePoint[] piecePoints;
    
    public Piece choicePiece;
    PiecePoint choicePiecePoint;   

    [SerializeField] public Team curTeam = Team.White;

    public bool isTurnEnd;
    public bool canClick = true;

    int pieceLayerMask;
    int boardLayerMask;

    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        pieceLayerMask = LayerMask.GetMask("Piece");
        boardLayerMask = LayerMask.GetMask("Board");

        promotionUI.UI.SetActive(false);
        InitPoint();
    }

    private void Start()
    {
        StartCoroutine(StartRoutine());
    }
    IEnumerator StartRoutine()
    {
        yield return Manager.Delay.ms05;
        ChessBoard.Instance.InitAttackTile();
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
        if (canClick == false) return;

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
                choicePiece.isClick = true;
                choicePiece.CheckOnWarningTile();

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
            choicePiece.isClick = false;

            BoardPos pointPos = ChessBoard.Instance.TransWorldToTile(choicePiecePoint.piecePoint.transform.position);
            if (choicePiece.CheckAbleTile(pointPos))
            {
                ReplacePiece(pointPos);           
                StartCoroutine(TurnEndRoutine());
            }
            else
            {
                choicePiece.RemoveAbleTile();
                choicePiecePoint.piecePoint.SetActive(false);
                choicePiece = null;
            }         
        }
    }
    Coroutine movePieceRoutine;
    IEnumerator MovePieceRoutine()
    {
        choicePiecePoint.piecePoint.SetActive(true);
        choicePiecePoint.piecePoint.transform.position = choicePiece.transform.position;
        while (true)
        {
            yield return Manager.Delay.ms05;
#if UNITY_EDITOR
            Vector3 touchPos = Input.mousePosition;
#else
            Vector3 touchPos = Input.touches[0].position;
#endif
            Ray ray = Camera.main.ScreenPointToRay(touchPos);
            Debug.DrawRay(ray.origin, ray.direction * 100f);
            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 100f, boardLayerMask))
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
        }
    }


    private void InitPoint()
    {
        piecePoints = new PiecePoint[(int)PieceType.Size];
        SetPoint(PieceType.Pawn, point.pawnPoint);
        SetPoint(PieceType.Knight, point.knightPoint);
        SetPoint(PieceType.Bishop, point.bishopPoint);
        SetPoint(PieceType.Rook, point.rookPoint);
        SetPoint(PieceType.Queen, point.queenPoint);
        SetPoint(PieceType.King, point.kingPoint);

        foreach (PiecePoint point in piecePoints)
        {
            point.piecePoint.SetActive(false);
        }
    }

    void SetPoint(PieceType type, GameObject point)
    {
        piecePoints[(int)type] = new PiecePoint();
        piecePoints[(int)type].piecePoint = point;
        piecePoints[(int)type].pointMaterial = piecePoints[(int)type].piecePoint.GetComponentInChildren<MeshRenderer>().material;
    }

    void ReplacePiece(BoardPos pointPos)
    {
        // �ش� ��ġ�� ��� �⹰�� �ִٸ� ��� �⹰�� ����
        if (ChessBoard.Instance.CheckTileOnBoard(pointPos, out PieceStruct enemyPiece))
        {
            if (enemyPiece.team != Team.Null && enemyPiece.team != choicePiece.team)
            {
                enemyPiece.piece.Die();
            }
        }

        // ���� ��ġ ����
        ChessBoard.Instance.UnPlacePiece(ChessBoard.Instance.TransWorldToTile(choicePiece.transform.position));
        choicePiece.RemoveAbleTile();
        // ���ο� ��ġ ���

        choicePiece.transform.position = choicePiecePoint.piecePoint.transform.position;
        PieceStruct piece = ChessBoard.GetPieceStruct(choicePiece);
        ChessBoard.Instance.PlacePiece(piece, ChessBoard.Instance.TransWorldToTile(choicePiece.transform.position));
        choicePiece.isMove = true;
        // �� �ѱ��
        if (choicePiece.team == Team.Black)
            curTeam = Team.White;
        else if (choicePiece.team == Team.White)
            curTeam = Team.Black;

        choicePiecePoint.piecePoint.SetActive(false);
        choicePiece = null;
    }

    Coroutine turnEndRoutine;

    IEnumerator TurnEndRoutine()
    {
        canClick = false;
        while (isTurnEnd == false)
        {
            yield return null;
        }
        ChessBoard.Instance.InitAttackTile();
        isTurnEnd = false;
        canClick = true;
    }
}
