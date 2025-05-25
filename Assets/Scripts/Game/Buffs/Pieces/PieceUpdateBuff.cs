namespace Assets.Scripts.Game.Buffs.Pieces
{
    using System;
    using System.Collections.Generic;
    using Assets.Scripts.Game.Board;
    using UnityEngine;

    // Buff that updates the piece's movement rules
    public class PieceUpdateBuff : PieceMoveBuff
    {
        public override Func<int, int, ChessBoard, bool, List<ChessTile>> MoveFunction { get; set; }

        public override string BuffName { get; set; };

        public override string Description { get; set; };

        public override Sprite Icon { get; set; }

        public override bool IsActive { get; set; } = true;

        public override int Cost { get; set; }

        public override int DurationTurns { get; set; }

        public override int DurationRounds { get; set; }

        public override bool WasUsed { get; set; }
    }
}
