namespace Assets.Scripts.Game.Buffs.Player.Update
{
    using System.Collections.Generic;
    using System.Linq;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AIShieldBuff", menuName = "Game/Buffs/AIShieldBuff")]
    public class AIShieldBuff : UpdateBuff
    {
        private readonly List<(int, ChessPiece)> previousHealthList = new();

        private readonly int maxHealth = 999;

        public AIShieldBuff()
        {
            this.UpdateFunction = this.AIShieldFnc;
        }

        public IChessObject AIShieldFnc(IChessObject chessObject)
        {
            var allTiles = ChessBoard.Instance.GetAllTiles();

            // Add temp max lives for this turn
            foreach (ChessTile tile in allTiles)
            {
                if (tile.CurrentPiece != null && tile.CurrentPiece.IsWhite)
                {
                    this.previousHealthList.Add((tile.CurrentPiece.Lives, tile.CurrentPiece));
                    tile.CurrentPiece.AddReduceLives(this.maxHealth, true);
                }
            }

            return null;
        }

        public override void RemoveBuff()
        {
            // Remove temp lives after turn is over

            foreach ((int previousLives, ChessPiece pieceToReduce) in this.previousHealthList)
            {
                pieceToReduce.AddReduceLives(previousLives - this.maxHealth, true);
            }
        }
    }
}
