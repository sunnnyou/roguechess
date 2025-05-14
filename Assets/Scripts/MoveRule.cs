// Represents the move logic and properties of a chess piece
using System;

[Serializable]
public class MoveRule
{
    public int xDirection;
    public int yDirection;
    public int maxDistance = 1; // 1 for moves like King, unlimited for Queen/Bishop etc.
    public bool canJumpOverPieces = false; // Knight is the only piece that can jump
}
