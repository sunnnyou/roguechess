using UnityEngine;

public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }
public enum Team { Player, Enemy }

public class ChessPiece : MonoBehaviour
{
    public PieceType type;
    public Team team;
    public Vector2Int boardPosition;

    public bool isAlive = true;

    public void MoveTo(Vector2Int newPos)
    {
        boardPosition = newPos;
        transform.position = new Vector3(newPos.x, newPos.y, 0);
    }
}
