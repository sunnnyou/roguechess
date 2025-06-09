namespace Assets.Scripts.Game.MoveRules
{
    using System.Collections.Generic;

    // Defines how pawns can move
    public class QueenMove : MoveRule
    {
        public static new List<MoveRule> GetMoveRules()
        {
            var queenMoves = new List<MoveRule>();
            queenMoves.AddRange(RookMove.GetMoveRules());
            queenMoves.AddRange(BishopMove.GetMoveRules());
            return queenMoves;
        }
    }
}
