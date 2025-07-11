namespace Assets.Scripts.Game.Buffs.Pieces.Move
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.MoveRules;
    using UnityEngine;

    [CreateAssetMenu(fileName = "SpeedRunBuff", menuName = "Game/Buffs/SpeedRunBuff")]
    public class SpeedRunBuff : MoveBuff
    {
        public SpeedRunBuff()
        {
            this.MoveFunction = SpeedRunFnc;
        }

        public static List<ChessTile> SpeedRunFnc(IChessObject chessObject)
        {
            var newValidTiles = new List<ChessTile>();
            if (chessObject == null || chessObject is not ChessPiece piece)
            {
                return newValidTiles;
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

            return MoveRule.GetValidTiles(addedMoves, piece.CurrentTile.Position, piece.IsWhite);
        }
    }
}
