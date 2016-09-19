using System;
using System.Collections.Generic;
using GuildInfo.Core.Models;

namespace GuildInfo.Core
{
    public interface IGuildService
    {
        IEnumerable<AggergatedCharacter> Fetch(string realm, string guildName);

        event EventHandler<CharacterLoadedEventArgs> CharacterLoaded;
    }
}
