namespace Assets.Scripts.UI.Buffs
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    [CreateAssetMenu(fileName = "BuffDatabase", menuName = "Game/Buff Database")]
    public class BuffDatabase : ScriptableObject
    {
        public Buff[] availableBuffs;

        public Buff GetRandomBuff()
        {
            if (availableBuffs.Length == 0)
                return null;
            return availableBuffs[Random.Range(0, availableBuffs.Length)];
        }
    }
}
