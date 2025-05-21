using UnityEngine;

public enum BuffType
{
    Passive,
    Temporary,
}

[CreateAssetMenu(fileName = "NewBuff", menuName = "ChessRoguelike/Buff")]
public class Buff : ScriptableObject
{
    public string BuffName;
    public BuffType BuffType;
    public string Description;
    public Sprite Icon;

    public bool IsOneTimeUse;
    public int Cost;

    // You can later add methods like:
    // public virtual void Apply(Player player) { }
}
