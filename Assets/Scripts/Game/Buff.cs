using UnityEngine;

public enum BuffType { Passive, Temporary }

[CreateAssetMenu(fileName = "NewBuff", menuName = "ChessRoguelike/Buff")]
public class Buff : ScriptableObject
{
    public string buffName;
    public BuffType buffType;
    public string description;
    public Sprite icon;

    public bool isOneTimeUse;
    public int cost;

    // You can later add methods like:
    // public virtual void Apply(Player player) { }
}
