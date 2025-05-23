namespace Assets.Scripts.Game.Board
{
    using UnityEngine;

    public class ChessMove
    {
        public Vector2Int StartPosition;
        public Vector2Int TargetPosition;
        public bool IsCapture;

        public ChessMove(Vector2Int start, Vector2Int target, bool capture = false)
        {
            this.StartPosition = start;
            this.TargetPosition = target;
            this.IsCapture = capture;
        }
    }
}
