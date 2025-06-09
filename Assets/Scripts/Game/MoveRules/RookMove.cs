namespace Assets.Scripts.Game.MoveRules
{
    using System.Collections.Generic;

    public class RookMove : MoveRule
    {
        public static new List<MoveRule> GetMoveRules()
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
    }
}
