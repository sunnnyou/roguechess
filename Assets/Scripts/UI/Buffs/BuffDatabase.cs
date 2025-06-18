namespace Assets.Scripts.UI.Buffs
{
    using System.Collections.Generic;
    using Assets.Scripts.Game.Buffs;
    using UnityEngine;

    [CreateAssetMenu(fileName = "BuffDatabase", menuName = "Game/Buff Database")]
    public class BuffDatabase : ScriptableObject
    {
        public IBuff[] AllBuffs;

        // Internal list to track remaining buffs
        private List<IBuff> remainingBuffs;

        private void OnEnable()
        {
            // Initialize the remainingBuffs list from allBuffs when the ScriptableObject is loaded
            this.ResetBuffPool();
        }

        public IBuff GetRandomBuff()
        {
            if (this.remainingBuffs == null || this.remainingBuffs.Count == 0)
            {
                return null;
            }

            int index = Random.Range(0, this.remainingBuffs.Count);
            IBuff selectedBuff = this.remainingBuffs[index];
            this.remainingBuffs.RemoveAt(index);
            return selectedBuff;
        }

        public void ResetBuffPool()
        {
            if (this.AllBuffs != null)
            {
                this.remainingBuffs = new List<IBuff>(this.AllBuffs);
            }
        }
    }
}
