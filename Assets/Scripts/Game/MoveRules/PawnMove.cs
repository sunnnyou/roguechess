namespace Assets.Scripts.Game.MoveRules
{
    using System.Collections.Generic;

    public class PawnMove : MoveRule
    {
        public static List<MoveRule> GetMoveRules(bool isWhite)
        {
            return new List<MoveRule>
            {
                new MoveRule
                {
                    XDirection = 0,
                    YDirection = isWhite ? 1 : -1,
                    MaxDistance = 1,
                    CannotCapture = true, // Pawns cannot move forward when capturing
                },
                new MoveRule
                {
                    XDirection = 1,
                    YDirection = isWhite ? 1 : -1,
                    MaxDistance = 1,
                    MustCapture = true, // Only captures diagonally
                },
                new MoveRule
                {
                    XDirection = -1,
                    YDirection = isWhite ? 1 : -1,
                    MaxDistance = 1,
                    MustCapture = true, // Only captures diagonally
                },
            };
        }
    }
}
