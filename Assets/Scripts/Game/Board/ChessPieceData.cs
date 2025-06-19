namespace Assets.Scripts.Game.Board
{
    using System.Collections.Generic;
    using Assets.Scripts.Game.Buffs;
    using Assets.Scripts.Game.MoveRules;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Chess Piece", menuName = "Chess/Chess Piece")]
    public class ChessPieceData : ScriptableObject
    {
        [Header("Piece Configuration")]
        public ChessPieceType PieceType;
        public bool IsWhite;
        public Sprite Sprite;
        public List<Material> Materials = new();
        public List<MoveRule> CustomMoveRules = new();
        public List<BuffBase> InitialBuffs = new();

        [Header("Combat Stats")]
        public int Strength = 1;
        public int Lives = 1;

        [Header("Rendering")]
        public int SortingOrder = 10;
    }
}
