namespace Assets.Scripts.Game.Board
{
    using System.Collections.Generic;
    using Assets.Scripts.Game.Buffs;
    using UnityEngine;

    public interface IChessObject
    {
        public abstract List<BuffBase> Buffs { get; }

        public SpriteRenderer SpriteRenderer { get; set; }
    }
}
