namespace Assets.Scripts.Game.Buffs.Player
{
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.MoveRules;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ZoomiesBuff", menuName = "Game/Buffs/ZoomiesBuff")]
    public class ZoomiesBuff : MoveBuff
    {
        private int? roundUsed;
        private readonly List<MoveRule> addedMoves = MoveRules.QueenMove.GetMoveRules();

        public ZoomiesBuff()
        {
            this.MoveFunction = this.ZoomiesFnc;
        }

        public List<ChessTile> ZoomiesFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for Zoomies buff.");
                return new List<ChessTile>();
            }

            if (
                piece.CurrentTile != null
                && piece.CurrentTile.Position != null
                && (this.roundUsed == null || this.roundUsed == ChessBoard.Instance.CurrentRound)
            )
            {
                this.roundUsed = ChessBoard.Instance.CurrentRound;
                return MoveRule.GetValidTiles(
                    this.addedMoves,
                    piece.CurrentTile.Position,
                    piece.IsWhite
                );
            }

            return new List<ChessTile>();
        }
    }
}
