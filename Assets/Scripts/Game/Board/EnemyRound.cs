namespace Assets.Scripts.Game.Board
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class EnemyRound
    {
        public string Name;
        public EnemyPiece[] Pieces;

        public EnemyRound(string name, EnemyPiece[] pieces)
        {
            this.Name = name;
            this.Pieces = pieces;
        }
    }
}
