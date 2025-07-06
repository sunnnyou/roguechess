namespace Assets.Scripts.Game.Buffs.Pieces.Move
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.MoveRules;
    using UnityEngine;

    [CreateAssetMenu(fileName = "SpeedRunBuff", menuName = "Game/Buffs/SpeedRunBuff")]
    public class SpeedRunBuff : UpdateBuff
    {
        public new string BuffName { get; set; }
        public new string Description { get; set; }
        public new Sprite Icon { get; set; }
        public new int Cost { get; set; }
        public new bool WasUsed { get; set; }

        public SpeedRunBuff()
        {
            this.UpdateFunction = SpeedRunFnc;
        }

        public static IChessObject SpeedRunFnc(IChessObject chessObject)
        {
            if (chessObject == null || chessObject is not ChessPiece piece)
            {
                return chessObject;
            }

            List<MoveRule> addedMoves = new();

            foreach (MoveRule moveRule in piece.MoveRules)
            {
                if (
                    moveRule.XDirection == 0 // left and right
                    || moveRule.YDirection == 0 // up and down
                    || Math.Abs(moveRule.YDirection) == Math.Abs(moveRule.YDirection) // diagonal
                )
                {
                    addedMoves.Add(
                        new MoveRule
                        {
                            XDirection = moveRule.XDirection,
                            YDirection = moveRule.YDirection,
                            MaxDistance = moveRule.MaxDistance + 1,
                            CanJumpOverPieces = moveRule.CanJumpOverPieces,
                        }
                    );
                }
            }

            return piece;
        }
    }
}
