namespace Assets.Scripts.UI.Buffs
{
    using System.Collections.Generic;
    using Assets.Scripts.Game.Buffs;
    using UnityEngine;

    [CreateAssetMenu(fileName = "BuffDatabase", menuName = "Game/Buff Database")]
    public class BuffDatabase : ScriptableObject
    {
        public BuffBase[] AllBuffs;

        private List<BuffBase> remainingBuffs;

        private void OnEnable()
        {
            // Initialize the remainingBuffs list from allBuffs when the ScriptableObject is loaded
            this.ResetBuffPool();
        }

        public BuffBase GetRandomBuff()
        {
            if (this.remainingBuffs == null || this.remainingBuffs.Count == 0)
            {
                return null;
            }

            int index = Random.Range(0, this.remainingBuffs.Count);
            BuffBase selected = this.remainingBuffs[index];
            this.remainingBuffs.RemoveAt(index);
            return selected;
        }

        public void ResetBuffPool()
        {
            if (this.AllBuffs != null)
            {
                this.remainingBuffs = new List<BuffBase>(this.AllBuffs);
            }
        }
    }
}
