using System.Collections.Generic;
using System.Linq;
using GuildInfo.Core.Models;

namespace GuildInfo.Wpf.ViewModels
{
    public class CharacterViewModel : ViewModelBase
    {
        public CharacterViewModel(GuildInfoCharacter character, string main) : this(character)
        {
            Main = main;
        }

        public CharacterViewModel(GuildInfoCharacter character, IEnumerable<CharacterViewModel> alts) : this(character)
        {
            Alts = alts;
        }

        public CharacterViewModel(GuildInfoCharacter character)
        {
            Name = character.Name;
            Class = character.Class;
            Level = character.Level;
            AverageItemLevelEquipped = character.AverageItemLevelEquipped;
            AverageItemLevel = character.AverageItemLevel;
            LegendaryItemsCount = character.LegendaryItemsCount;
            ArtifactLevel = character.ArtifactLevel;
            Icon = string.Format(GuildViewModel.CLASS_IMAGE_TEMPLATE, character.Class);
            RingEnchant1 = character.RingEnchant1.ToString();
            RingEnchant2 = character.RingEnchant2.ToString();
            NeckEnchant = character.NeckEnchant.ToString();
            CloakEnchant = character.CloakEnchant.ToString();
            GemInfo = $"{character.GemInfo.EpicGems}/{character.GemInfo.TopGems}/{character.GemInfo.CheapGems}";
        }
        
        public string Name { get; private set; }
        public int Class { get; private set; }
        public int Level { get; private set; }
        public int AverageItemLevelEquipped { get; private set; }
        public int AverageItemLevel { get; private set; }
        public int LegendaryItemsCount { get; private set; }
        public int ArtifactLevel { get; private set; }
        public string Icon { get; private set; }

        public string RingEnchant1 { get; private set; }
        public string RingEnchant2 { get; private set; }
        public string NeckEnchant { get; private set; }
        public string CloakEnchant { get; private set; }
        public string GemInfo { get; private set; }

        public string Main { get; private set; }
        public IEnumerable<CharacterViewModel> Alts { get; }

        public int AltsCount
        {
            get { return Alts?.Count() ?? 0; }
        }

    }
}
