using System;

namespace GuildInfo.Core.Models
{
    public class CharacterLoadedEventArgs : EventArgs
    {
        public CharacterLoadedEventArgs(int charactersTotal)
        {
            CharactersTotal = charactersTotal;
        }

        public int CharactersTotal { get; private set; }
    }
}
