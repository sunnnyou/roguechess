using System.Collections.Generic;
using Assets.Scripts.Game.Board;
using UnityEngine;

[CreateAssetMenu(fileName = "New Board Setup", menuName = "Chess/Board Setup")]
public class ChessBoardData : ScriptableObject
{
    [Header("Board Dimensions")]
    public int BoardWidth = 8;
    public int BoardHeight = 8;
    public float TileSize = 1.0f;
    public bool AutoScale = true;
    public float ScalePadding = 0.05f;

    [Header("Special Tile Positions")]
    public List<ChessTileData> Tiles = new();

    public ChessTileData GetTileDataAt(int x, int y)
    {
        var pos = new Vector2Int(x, y);
        var specialTile = this.Tiles.Find(t => t.Position == pos);
        return specialTile;
    }
}
