namespace Assets.Scripts.Game.Buffs.Player.Update
{
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using Assets.Scripts.Game.Buffs.Tiles.Update;
    using UnityEngine;

    [CreateAssetMenu(fileName = "UnoReverseBuff", menuName = "Game/Buffs/UnoReverseBuff")]
    public class UnoReverseBuff : UpdateBuff
    {
        private ChessTile currentTile;

        public UnoReverseBuff()
        {
            this.UpdateFunction = this.UnoReverseFnc;
        }

        public IChessObject UnoReverseFnc(IChessObject chessObject)
        {
            if (chessObject is not ChessPiece piece || piece == null)
            {
                Debug.LogError("Invalid arguments for UnoReverseFnc buff.");
                return null;
            }

            if (this.currentTile == null)
            {
                this.currentTile = piece.CurrentTile;
                this.currentTile.AddBuff(new UnoReversTileBuff(piece));
            }
            else if (this.currentTile.Position.ToString() != piece.CurrentTile.ToString())
            {
                this.currentTile = piece.CurrentTile;
            }

            return null;
        }

        public override void RemoveBuff()
        {
            // TODO: Remove buff from tile
        }
    }
}
