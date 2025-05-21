// Represents the move logic and properties of a chess piece
using System;

[Serializable]
public class MoveRule
{
    public int XDirection;
    public int YDirection;
    public int MaxDistance = 1; // 1 for moves like King, unlimited for Queen/Bishop etc.
    public bool CanJumpOverPieces; // Knight is the only piece that can jump
}
