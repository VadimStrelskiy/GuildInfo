﻿using System;
using System.Linq;
using System.Text;
using WowDotNetAPI.Models;

namespace GuildInfo.Core.Models
{
    public class GuildInfoCharacter
    {
        private const int AP_ACHIEVEMNT = 29395;

        public GuildInfoCharacter(Character character, GuildMember member)
        {
            if(character == null) throw new ArgumentNullException(nameof(character));
            if(member == null) throw new ArgumentNullException(nameof(member));

            Name = character.Name;
            Level = character.Level;
            GuildRank = member.Rank;
            Class = (int)character.Class;
            AchievementPoints = character.AchievementPoints;
            AverageItemLevelEquipped = character.Items.AverageItemLevelEquipped;
            AverageItemLevel = character.Items.AverageItemLevel;
            ArtifactLevel = CalculateArtifactLevelTrueCount(character);
            LegendaryItemsCount = CalculateLegendaryItemsCount(character.Items);

            RingEnchant1 = ItemImprovementHelper.GetRingEnchantQuality(character.Items.Finger1);
            RingEnchant2 = ItemImprovementHelper.GetRingEnchantQuality(character.Items.Finger2);
            NeckEnchant = ItemImprovementHelper.GetNeckEnchantQuality(character.Items.Neck);
            CloakEnchant = ItemImprovementHelper.GetCloakEnchantQuality(character.Items.Back);
            GemInfo = ItemImprovementHelper.GetGemInfo(character.Items);

            var sb = new StringBuilder();
            foreach (var pet in character.Pets.Collected)
            {
                sb.Append(pet.CreatureId + "_");
            }
            Pets = sb.ToString();
        }

        public string Name { get; private set; }
        public int Level { get; private set; }
        public int GuildRank { get; private set; }
        public int AverageItemLevelEquipped { get; private set; }
        public int AverageItemLevel { get; private set; }
        public int LegendaryItemsCount { get; private set; }
        public int ArtifactLevel { get; private set; }
        public int Class { get; private set; }
        public EnchantQuality RingEnchant1 { get; private set; }
        public EnchantQuality RingEnchant2 { get; private set; }
        public EnchantQuality NeckEnchant { get; private set; }
        public EnchantQuality CloakEnchant { get; private set; }
        public GemInfo GemInfo { get; private set; }

        internal int AchievementPoints { get; private set; }
        internal string Pets { get; private set; }

        private static int CalculateArtifactLevelTrueCount(Character character)
        {
            var pos = character.Achievements.Criteria.ToList().IndexOf(AP_ACHIEVEMNT);
            if (pos == -1) return 0;
            return (int)(character.Achievements.CriteriaQuantity.ToList()[pos]);
            //CharacterItem artifact = null;
            //var mainHand = character.Items.MainHand;
            //var offHand = character.Items.OffHand;
            //if (mainHand?.ArtifactTraits != null && mainHand.ArtifactTraits.Any())
            //{
            //    artifact = mainHand;

            //}
            //else if (offHand?.ArtifactTraits != null && offHand.ArtifactTraits.Any())
            //{
            //    artifact = offHand;
            //}

            //if (artifact == null)
            //{
            //    return -1;
            //}

            //var artifactLevel = artifact.ArtifactTraits.Sum(at => at.Rank);
            //if (artifact.Relics != null && artifact.Relics.Any())
            //{
            //    artifactLevel -= artifact.Relics.Count();
            //}

            //return artifactLevel;
        }

        private static int CalculateLegendaryItemsCount(CharacterEquipment items)
        {
            int legendaryItemsCount = 0;
            legendaryItemsCount += IsItemLegendary(items.Legs);
            legendaryItemsCount += IsItemLegendary(items.MainHand);
            legendaryItemsCount += IsItemLegendary(items.Back);
            legendaryItemsCount += IsItemLegendary(items.Chest);
            legendaryItemsCount += IsItemLegendary(items.Feet);
            legendaryItemsCount += IsItemLegendary(items.Finger1);
            legendaryItemsCount += IsItemLegendary(items.Finger2);
            legendaryItemsCount += IsItemLegendary(items.Hands);
            legendaryItemsCount += IsItemLegendary(items.Head);
            legendaryItemsCount += IsItemLegendary(items.Neck);
            legendaryItemsCount += IsItemLegendary(items.OffHand);
            legendaryItemsCount += IsItemLegendary(items.Shirt);
            legendaryItemsCount += IsItemLegendary(items.Ranged);
            legendaryItemsCount += IsItemLegendary(items.Shoulder);
            legendaryItemsCount += IsItemLegendary(items.Tabard);
            legendaryItemsCount += IsItemLegendary(items.Trinket1);
            legendaryItemsCount += IsItemLegendary(items.Trinket2);
            legendaryItemsCount += IsItemLegendary(items.Waist);
            legendaryItemsCount += IsItemLegendary(items.Wrist);

            return legendaryItemsCount;
        }

        private static int IsItemLegendary(CharacterItem item)
        {
            return item?.Quality == 5 && item.ItemLevel >= 895 ? 1 : 0;
        }
    }
}
