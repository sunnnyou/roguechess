namespace Assets.Scripts.Game.Board
{
    using System.Collections.Generic;
    using Assets.Scripts.Game.Buffs;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Chess Tile", menuName = "Chess/Chess Tile")]
    public class ChessTileData : ScriptableObject
    {
        [Header("Tile Configuration")]
        public Sprite Sprite;
        public Color WhiteColor = Color.white;
        public Color BlackColor = Color.black;
        public List<BuffBase> InitialBuffs = new();
        public Vector2Int Position;

        [Header("Visual Settings")]
        public Color HighlightColor = new(0.0f, 0.4f, 0.0f); // green
        public int SortingOrder = 2;

        [Header("Starting Piece")]
        public ChessPieceData StartingPiece;
        public bool SpawnPieceOnInitialize;

        [Header("Special Tile Properties")]
        public bool HasSpecialProperties = false;

        [TextArea(3, 5)]
        public string Description = string.Empty;
    }
}
