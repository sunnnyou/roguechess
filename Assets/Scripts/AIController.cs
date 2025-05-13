using System.Collections;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public IEnumerator PlayEnemyTurn()
    {
        yield return new WaitForSeconds(1f);

        // TODO: Replace with smarter logic
        Debug.Log("Enemy turn logic not implemented.");

        TurnManager.Instance.EndTurn();
    }
}
