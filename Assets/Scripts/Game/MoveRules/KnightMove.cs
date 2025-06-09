namespace Assets.Scripts.Game.MoveRules
{
    using System.Collections.Generic;

    public class KnightMove : MoveRule
    {
        public static new List<MoveRule> GetMoveRules()
        {
            return new List<MoveRule>
            {
                new MoveRule
                {
                    XDirection = 2,
                    YDirection = 1,
                    MaxDistance = 1,
                    CanJumpOverPieces = true,
                },
                new MoveRule
                {
                    XDirection = 2,
                    YDirection = -1,
                    MaxDistance = 1,
                    CanJumpOverPieces = true,
                },
                new MoveRule
                {
                    XDirection = -2,
                    YDirection = 1,
                    MaxDistance = 1,
                    CanJumpOverPieces = true,
                },
                new MoveRule
                {
                    XDirection = -2,
                    YDirection = -1,
                    MaxDistance = 1,
                    CanJumpOverPieces = true,
                },
                new MoveRule
                {
                    XDirection = 1,
                    YDirection = 2,
                    MaxDistance = 1,
                    CanJumpOverPieces = true,
                },
                new MoveRule
                {
                    XDirection = 1,
                    YDirection = -2,
                    MaxDistance = 1,
                    CanJumpOverPieces = true,
                },
                new MoveRule
                {
                    XDirection = -1,
                    YDirection = 2,
                    MaxDistance = 1,
                    CanJumpOverPieces = true,
                },
                new MoveRule
                {
                    XDirection = -1,
                    YDirection = -2,
                    MaxDistance = 1,
                    CanJumpOverPieces = true,
                },
            };
        }
    }
}
