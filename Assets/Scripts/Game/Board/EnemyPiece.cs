namespace Assets.Scripts.Game.Board
{
    [System.Serializable]
    public struct EnemyPiece
    {
        public ChessPieceType PieceType;
        public string Position; // "e8", "f6", etc.

        public EnemyPiece(ChessPieceType type, string pos)
        {
            this.PieceType = type;
            this.Position = pos;
        }

        public override readonly string ToString() => this.PieceType + "@" + this.Position;

        public override readonly bool Equals(object obj)
        {
            return obj is EnemyPiece piece && piece.ToString() == this.ToString();
        }
    }
}
