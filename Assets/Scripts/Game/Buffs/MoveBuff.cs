namespace Assets.Scripts.Game.Buffs
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    // Move buffs add valid moves to a chess piece
    public abstract class MoveBuff : BuffBase
    {
        public Func<ChessPiece, List<ChessTile>> MoveFunction { get; set; } // Function that is applied when a condition is met

        internal override object BuffFunction(IChessObject buffReceiver)
        {
            if (buffReceiver is not ChessPiece piece)
            {
                Debug.LogError(
                    $"Invalid arguments for MoveBuff '{this.BuffName}'. Expected ChessPiece for buffReceiver."
                );
                return null;
            }

            return this.MoveFunction(piece);
        }
    }
}
