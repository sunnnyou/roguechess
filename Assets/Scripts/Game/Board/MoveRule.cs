namespace Assets.Scripts.Game.Board
{
    // Defines how pieces can move
    [System.Serializable]
    public class MoveRule
    {
        public int XDirection;
        public int YDirection;
        public int MaxDistance = 1;
        public bool CanJumpOverPieces;
        public bool MustCapture; // For pieces that can only move when capturing
        public bool CannotCapture; // For pieces that cannot capture (like pawn forward movement)
    }
}
