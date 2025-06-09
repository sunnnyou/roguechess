namespace Assets.Scripts.Game.MoveRules
{
    using System.Collections.Generic;

    public class KingMove : MoveRule
    {
        public static new List<MoveRule> GetMoveRules()
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
