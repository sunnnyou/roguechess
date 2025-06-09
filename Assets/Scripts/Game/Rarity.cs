namespace Assets.Scripts.Game
{
    using System;

    // Represents the rarity of a game item
    [Serializable]
    public enum RarityEnum
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythic,
    }

    // Extension methods for Rarity
    public static class Rarity
    {
        public static string ToDisplayString(this RarityEnum rarity)
        {
            return rarity switch
            {
                RarityEnum.Common => "Common",
                RarityEnum.Uncommon => "Uncommon",
                RarityEnum.Rare => "Rare",
                RarityEnum.Epic => "Epic",
                RarityEnum.Legendary => "Legendary",
                RarityEnum.Mythic => "Mythic",
                _ => "Unknown",
            };
        }
    }
}
