namespace Assets.Scripts.Game.Board
{
    using System.Collections.Generic;

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

        public static List<MoveRule> Pawn(bool isWhite)
        {
            return new List<MoveRule>
            {
                new MoveRule
                {
                    XDirection = 0,
                    YDirection = isWhite ? 1 : -1,
                    MaxDistance = 1,
                },
            };
        }

        public static List<MoveRule> Rook()
        {
            return new List<MoveRule>
            {
                new MoveRule
                {
                    XDirection = 1,
                    YDirection = 0,
                    MaxDistance = int.MaxValue,
                },
                new MoveRule
                {
                    XDirection = -1,
                    YDirection = 0,
                    MaxDistance = int.MaxValue,
                },
                new MoveRule
                {
                    XDirection = 0,
                    YDirection = 1,
                    MaxDistance = int.MaxValue,
                },
                new MoveRule
                {
                    XDirection = 0,
                    YDirection = -1,
                    MaxDistance = int.MaxValue,
                },
            };
        }

        public static List<MoveRule> Knight()
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

        public static List<MoveRule> Bishop()
        {
            return new List<MoveRule>
            {
                new MoveRule
                {
                    XDirection = 1,
                    YDirection = 1,
                    MaxDistance = int.MaxValue,
                },
                new MoveRule
                {
                    XDirection = -1,
                    YDirection = 1,
                    MaxDistance = int.MaxValue,
                },
                new MoveRule
                {
                    XDirection = 1,
                    YDirection = -1,
                    MaxDistance = int.MaxValue,
                },
                new MoveRule
                {
                    XDirection = -1,
                    YDirection = -1,
                    MaxDistance = int.MaxValue,
                },
            };
        }

        public static List<MoveRule> Queen()
        {
            var queenMoves = new List<MoveRule>();
            queenMoves.AddRange(Rook());
            queenMoves.AddRange(Bishop());
            return queenMoves;
        }

        public static List<MoveRule> King()
        {
            return new List<MoveRule>
            {
                new MoveRule
                {
                    XDirection = 1,
                    YDirection = 0,
                    MaxDistance = 1,
                },
                new MoveRule
                {
                    XDirection = -1,
                    YDirection = 0,
                    MaxDistance = 1,
                },
                new MoveRule
                {
                    XDirection = 0,
                    YDirection = 1,
                    MaxDistance = 1,
                },
                new MoveRule
                {
                    XDirection = 0,
                    YDirection = -1,
                    MaxDistance = 1,
                },
                new MoveRule
                {
                    XDirection = 1,
                    YDirection = 1,
                    MaxDistance = 1,
                },
                new MoveRule
                {
                    XDirection = -1,
                    YDirection = 1,
                    MaxDistance = 1,
                },
                new MoveRule
                {
                    XDirection = 1,
                    YDirection = -1,
                    MaxDistance = 1,
                },
                new MoveRule
                {
                    XDirection = -1,
                    YDirection = -1,
                    MaxDistance = 1,
                },
            };
        }
    }
}
