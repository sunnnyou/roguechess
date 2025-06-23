namespace Assets.Scripts.Game.Buffs.Pieces.Move
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.Buffs.Tiles.Update;
    using UnityEngine;

    [CreateAssetMenu(fileName = "SpeedRunBuff", menuName = "Game/Buffs/SpeedRunBuff")]
    public class SpeedRunBuff : MoveBuff
    {
        public new string BuffName { get; set; } = "SpeedRun";

        public new string Description { get; set; } =
            "Gives a piece the ability to use en passant.";

        public new Sprite Icon { get; set; }

        public new int Cost { get; set; }

        public new bool WasUsed { get; set; }

        public SpeedRunBuff()
        {
            this.MoveFunction = SpeedRunFnc;
        }

        public static List<ChessTile> SpeedRunFnc(Vector2Int currentPos, bool isWhite)
        {
            var validMoves = new List<ChessTile>();
            if (
                (!isWhite || currentPos.y != 1)
                && (isWhite || currentPos.y != ChessBoard.Instance.Height - 2)
            )
            {
                return validMoves;
            }

            // TODO:

            int forwardDirection = isWhite ? 1 : -1;
            int twoForward = currentPos.y + (forwardDirection * 2);
            if (twoForward < 0 || twoForward >= ChessBoard.Instance.Height)
            {
                return validMoves;
            }

            Vector2Int oneStepCoord = CoordinateHelper.XYToVector(
                currentPos.x,
                currentPos.y + forwardDirection
            );
            Vector2Int twoStepCoord = CoordinateHelper.XYToVector(currentPos.x, twoForward);

            if (!ChessBoard.Instance.GetTile(oneStepCoord, out ChessTile oneStepTile))
            {
                return validMoves;
            }

            if (!ChessBoard.Instance.GetTile(twoStepCoord, out ChessTile twoStepTile))
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
