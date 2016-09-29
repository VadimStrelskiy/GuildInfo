using System;
using System.Collections.Generic;
using System.Linq;
using WowDotNetAPI.Models;

namespace GuildInfo.Core.Models
{
    internal static class ItemImprovementHelper
    {
        private static readonly IEnumerable<int> TopRingEnchants = new List<int>
        {
            5427, 5428, 5429, 5430
        };

        private static readonly IEnumerable<int> CheapRingEnchants = new List<int>
        {
            5423, 5424, 5425, 5436
        };

        private static readonly IEnumerable<int> NeckEnchants = new List<int>
        {
            5437, 5438, 5439, 5889, 5890, 5891
        };

        private static readonly IEnumerable<int> TopCloakEnchants = new List<int>
        {
            5434, 5435, 5436
        };

        private static readonly IEnumerable<int> CheapCloakEnchants = new List<int>
        {
            5431, 5432, 5433
        };

        private static readonly IEnumerable<int> EpicGems = new List<int>
        {
            130246, 130247, 130248
        };

        private static readonly IEnumerable<int> TopGems = new List<int>
        {
            130219, 130220, 130221, 130222
        };

        private static readonly IEnumerable<int> CheapGems = new List<int>
        {
            130215, 130216, 130217, 130218
        };

        public static EnchantQuality GetRingEnchantQuality(CharacterItem item)
        {
            if (item == null) return EnchantQuality.None;
            var enchant = item.TooltipParams.Enchant;
            if(TopRingEnchants.Contains(enchant)) return EnchantQuality.Top;
            if(CheapRingEnchants.Contains(enchant)) return EnchantQuality.Cheap;
            return EnchantQuality.None;
        }

        public static EnchantQuality GetCloakEnchantQuality(CharacterItem item)
        {
            if (item == null) return EnchantQuality.None;
            var enchant = item.TooltipParams.Enchant;
            if (TopCloakEnchants.Contains(enchant)) return EnchantQuality.Top;
            if (CheapCloakEnchants.Contains(enchant)) return EnchantQuality.Cheap;
            return EnchantQuality.None;
        }

        public static EnchantQuality GetNeckEnchantQuality(CharacterItem item)
        {
            if (item == null) return EnchantQuality.None;
            var enchant = item.TooltipParams.Enchant;
            if (NeckEnchants.Contains(enchant)) return EnchantQuality.Top;
            return EnchantQuality.None;
        }

        public static GemInfo GetGemInfo(CharacterEquipment equipment)
        {
            var gemInfo = new GemInfo();
            UpdateGemInfo(equipment.Legs, gemInfo);
            UpdateGemInfo(equipment.MainHand, gemInfo);
            UpdateGemInfo(equipment.Back, gemInfo);
            UpdateGemInfo(equipment.Chest, gemInfo);
            UpdateGemInfo(equipment.Feet, gemInfo);
            UpdateGemInfo(equipment.Finger1, gemInfo);
            UpdateGemInfo(equipment.Finger2, gemInfo);
            UpdateGemInfo(equipment.Hands, gemInfo);
            UpdateGemInfo(equipment.Head, gemInfo);
            UpdateGemInfo(equipment.Neck, gemInfo);
            UpdateGemInfo(equipment.OffHand, gemInfo);
            UpdateGemInfo(equipment.Shirt, gemInfo);
            UpdateGemInfo(equipment.Ranged, gemInfo);
            UpdateGemInfo(equipment.Shoulder, gemInfo);
            UpdateGemInfo(equipment.Tabard, gemInfo);
            UpdateGemInfo(equipment.Trinket1, gemInfo);
            UpdateGemInfo(equipment.Trinket2, gemInfo);
            UpdateGemInfo(equipment.Waist, gemInfo);
            UpdateGemInfo(equipment.Wrist, gemInfo);

            return gemInfo;
        }

        private static void UpdateGemInfo(CharacterItem item, GemInfo gemInfo)
        {
            if(item == null) return;
            UpdateGemInfo(GetGemQuality(item.TooltipParams.Gem0), gemInfo);
            UpdateGemInfo(GetGemQuality(item.TooltipParams.Gem1), gemInfo);
            UpdateGemInfo(GetGemQuality(item.TooltipParams.Gem2), gemInfo);
        }

        private static void UpdateGemInfo(GemQuality? gemQuality, GemInfo gemInfo)
        {
            if (gemQuality == null) return;
            switch (gemQuality)
            {
                case GemQuality.Epic:
                    gemInfo.EpicGems ++;
                    break;
                case GemQuality.Top:
                    gemInfo.TopGems++;
                    break;
                case GemQuality.Cheap:
                    gemInfo.CheapGems++;
                    break;
            }
        }

        private static GemQuality? GetGemQuality(int id)
        {
            if (id == 0) return null;
            if (EpicGems.Contains(id)) return GemQuality.Epic;
            if (TopGems.Contains(id)) return GemQuality.Top;
            if (CheapGems.Contains(id)) return GemQuality.Cheap;
            return null;
        }

        private enum GemQuality
        {
            Epic,
            Top,
            Cheap
        }
    }
}
