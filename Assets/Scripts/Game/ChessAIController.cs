using System.Collections;
using UnityEngine;

public class ChessAIController : MonoBehaviour
{
    public ChessBoard chessBoard;
    public bool isWhiteAI = false;
    public int searchDepth = 3;
    public float moveDelay = 1f;

    private ChessAI ai;
    private float moveTimer = 0f;
    private bool isProcessingMove = false;

    void Start()
    {
        if (chessBoard == null)
        {
            chessBoard = FindFirstObjectByType<ChessBoard>();
        }
        ai = new ChessAI(chessBoard, isWhiteAI, searchDepth);
    }

    void Update()
    {
        if (chessBoard == null || chessBoard.IsGameOver() || isProcessingMove)
            return;

        if (chessBoard.IsWhiteTurn() == isWhiteAI)
        {
            moveTimer += Time.deltaTime;
            if (moveTimer >= moveDelay)
            {
                StartCoroutine(ProcessAIMove());
            }
        }
        else
        {
            moveTimer = 0f;
        }
    }

    private IEnumerator ProcessAIMove()
    {
        isProcessingMove = true;
        chessBoard.SetAITurn(true);

        yield return new WaitForSeconds(0.5f); // Add slight delay for visual feedback

        ai.MakeMove();
        moveTimer = 0f;

        yield return new WaitForSeconds(0.5f); // Add slight delay after move

        chessBoard.SetAITurn(false);
        isProcessingMove = false;
    }

    void OnValidate()
    {
        if (ai != null)
        {
            ai = new ChessAI(chessBoard, isWhiteAI, searchDepth);
        }
    }
}
