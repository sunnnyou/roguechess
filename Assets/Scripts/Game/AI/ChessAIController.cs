namespace Assets.Scripts.Game.AI
{
    using System.Collections;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    public class ChessAIController : MonoBehaviour
    {
        public ChessBoard ChessBoard;
        public bool IsWhiteAI;
        public int SearchDepth = 3;
        public float MoveDelay = 1f;

        private ChessAI ai;
        private float moveTimer;
        private bool isProcessingMove;

        public void Start()
        {
            if (this.ChessBoard == null)
            {
                this.ChessBoard = FindFirstObjectByType<ChessBoard>();
            }

            this.ai = new ChessAI(this.ChessBoard, this.IsWhiteAI, this.SearchDepth);
        }

        public void Update()
        {
            if (this.ChessBoard == null || this.ChessBoard.IsGameOver() || this.isProcessingMove)
            {
                return;
            }

            if (this.ChessBoard.IsWhiteTurn() == this.IsWhiteAI)
            {
                this.moveTimer += Time.deltaTime;
                if (this.moveTimer >= this.MoveDelay)
                {
                    this.StartCoroutine(this.ProcessAIMove());
                }
            }
            else
            {
                this.moveTimer = 0f;
            }
        }

        private IEnumerator ProcessAIMove()
        {
            this.isProcessingMove = true;
            this.ChessBoard.SetAITurn(true);

            yield return new WaitForSeconds(0.5f); // Add slight delay for visual feedback

            this.ai.MakeMove();
            this.moveTimer = 0f;

            yield return new WaitForSeconds(0.5f); // Add slight delay after move

            this.ChessBoard.SetAITurn(false);
            this.isProcessingMove = false;
        }

        public void OnValidate()
        {
            if (this.ai != null)
            {
                this.ai = new ChessAI(this.ChessBoard, this.IsWhiteAI, this.SearchDepth);
            }
        }
    }
}
