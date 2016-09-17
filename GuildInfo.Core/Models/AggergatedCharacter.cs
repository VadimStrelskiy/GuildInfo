using System.Collections.Generic;

namespace GuildInfo.Core.Models
{
    public class AggergatedCharacter
    {
        public GuildInfoCharacter Main { get; set; }

        public IEnumerable<GuildInfoCharacter> Alts { get; set; }
    }
}
