using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text coinsText;
    public TMP_Text turnText;
    public TMP_Text buffsText;

    public void UpdateUI(int coins, int turns, string buffs)
    {
        coinsText.text = $"Coins: {coins}";
        turnText.text = $"Turns: {turns}";
        buffsText.text = $"Buffs: {buffs}";
    }
}
