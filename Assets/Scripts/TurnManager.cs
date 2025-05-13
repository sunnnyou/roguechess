using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public bool playerTurn = true;

    private void Awake()
    {
        Instance = this;
    }

    public void EndTurn()
    {
        playerTurn = !playerTurn;

        if (!playerTurn)
        {
            StartCoroutine(FindObjectOfType<AIController>().PlayEnemyTurn());
        }
    }
}
