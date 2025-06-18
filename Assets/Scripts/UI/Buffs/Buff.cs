namespace Assets.Scripts.UI.Buffs
{
    using UnityEngine;

    [System.Serializable]
    public class Buff
    {
        public string title;

        [TextArea(3, 5)]
        public string description;
        public int cost;
        public Sprite image;
    }
}
