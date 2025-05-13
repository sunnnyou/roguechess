using UnityEngine;
using System.Collections.Generic;

public class ChessBoardManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject[] piecePrefabs; // Assign prefabs in order of PieceType

    public int boardWidth = 8;
    public int boardHeight = 8;

    public ChessPiece[,] board;

    void Start()
    {
        GenerateBoard();
    }

    public void GenerateBoard()
    {
        board = new ChessPiece[boardWidth, boardHeight];

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                Instantiate(tilePrefab, new Vector3(x, y, 1), Quaternion.identity, transform);
            }
        }
    }

    public void SpawnPiece(PieceType type, Team team, Vector2Int position)
    {
        GameObject prefab = piecePrefabs[(int)type];
        GameObject pieceGO = Instantiate(prefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
        ChessPiece piece = pieceGO.GetComponent<ChessPiece>();
        piece.type = type;
        piece.team = team;
        piece.boardPosition = position;

        board[position.x, position.y] = piece;
    }

    public void ClearBoard()
    {
        foreach (ChessPiece piece in board)
        {
            if (piece != null)
                Destroy(piece.gameObject);
        }
    }
}
