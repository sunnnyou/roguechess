using UnityEngine;

public class ChessMove
{
    public Vector2Int startPosition;
    public Vector2Int targetPosition;
    public bool isCapture;

    public ChessMove(Vector2Int start, Vector2Int target, bool capture = false)
    {
        startPosition = start;
        targetPosition = target;
        isCapture = capture;
    }
}
