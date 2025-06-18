namespace Assets.Scripts.Game.Buffs.Pieces.Move
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    // Buff that allows a piece to move twice in one turn
    [CreateAssetMenu(fileName = "ExtraReachPieceBuff", menuName = "Game/Buffs/ExtraReachPieceBuff")]
    public class ExtraReachPieceBuff : MoveBuff
    {
        public string BuffName { get; set; } = "Extra Reach";

        public string Description { get; set; } =
            "Allows a piece to move to additional tiles in one turn.";

        public Sprite Icon { get; set; }

        public int Cost { get; set; }

        public bool WasUsed { get; set; }

        private readonly int additionalTiles;

        private readonly bool forwardOnly;

        public ExtraReachPieceBuff(
            bool forwardOnly = true,
            int additionalTiles = 1,
            int durationTurns = 1
        )
        {
            this.forwardOnly = forwardOnly;
            this.DurationTurns = durationTurns;
            this.additionalTiles = additionalTiles;
            this.MoveFunction = DoubleMove;
        }

        public static List<ChessTile> DoubleMove(
            Vector2Int currentPos,
            ChessBoard board,
            bool isWhite
        )
        {
            var validMoves = new List<ChessTile>();
            if (
                board == null
                || (
                    (!isWhite || currentPos.y != 1) && (isWhite || currentPos.y != board.Height - 2)
                )
            )
            {
                return validMoves;
            }

            // TODO: Implement logic for forward-only moves and additional tiles

            int forwardDirection = isWhite ? 1 : -1;
            int twoForward = currentPos.y + (forwardDirection * 2);
            if (twoForward < 0 || twoForward >= board.Height)
            {
                return validMoves;
            }

            Vector2Int oneStepCoord = CoordinateHelper.XYToVector(
                currentPos.x,
                currentPos.y + forwardDirection
            );
            Vector2Int twoStepCoord = CoordinateHelper.XYToVector(currentPos.x, twoForward);

            if (!board.GetTile(oneStepCoord, out ChessTile oneStepTile))
            {
                return validMoves;
            }

            if (!board.GetTile(twoStepCoord, out ChessTile twoStepTile))
            {
                return validMoves;
            }

            if (oneStepTile.CurrentPiece == null && twoStepTile.CurrentPiece == null)
            {
                validMoves.Add(twoStepTile);
            }

            return validMoves;
        }
    }
}
