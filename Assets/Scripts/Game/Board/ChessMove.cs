namespace Assets.Scripts.Game.Board
{
    using UnityEngine;

    // Represents a chess move
    [System.Serializable]
    public class ChessMove
    {
        public Vector2Int StartPosition;
        public Vector2Int TargetPosition;
        public bool IsCapture;
        public float Score; // For AI evaluation

        public ChessMove(Vector2Int start, Vector2Int target, bool capture = false)
        {
            this.StartPosition = start;
            this.TargetPosition = target;
            this.IsCapture = capture;
            this.Score = 0f;
        }

        public ChessMove(Vector2Int start, Vector2Int target, bool capture, float score)
        {
            this.StartPosition = start;
            this.TargetPosition = target;
            this.IsCapture = capture;
            this.Score = score;
        }

        public override string ToString()
        {
            return $"Move from {this.StartPosition} to {this.TargetPosition} (Capture: {this.IsCapture}, Score: {this.Score:F2})";
        }
    }
}
