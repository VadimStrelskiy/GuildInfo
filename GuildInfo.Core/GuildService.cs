using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GuildInfo.Core.Models;
using WowDotNetAPI;
using WowDotNetAPI.Models;

namespace GuildInfo.Core
{
    public class GuildService : IGuildService
    {
        private readonly WowExplorer _explorer;

        public event EventHandler<CharacterLoadedEventArgs> CharacterLoaded;

        public GuildService(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            _explorer = new WowExplorer(Region.EU, Locale.ru_RU, apiKey);
        }

        public IEnumerable<AggergatedCharacter> Fetch(string realmName, string guildName)
        {
            if (string.IsNullOrWhiteSpace(realmName)) throw new ArgumentNullException(nameof(realmName));
            if (string.IsNullOrWhiteSpace(guildName)) throw new ArgumentNullException(nameof(guildName));

            var guild = GetGuildInternal(realmName, guildName);

            var characters = new List<GuildInfoCharacter>();

#if DEBUG
            //guild.Members = guild.Members.Take(5).ToList();
#endif

            Task.WaitAll(guild.Members.Select(member => Task.Factory.StartNew(() =>
            {
                var character = GetCharacterInternal(member);
                if (character == null) return;
                characters.Add(new GuildInfoCharacter(character, member));
                CharacterLoaded?.Invoke(this, new CharacterLoadedEventArgs(guild.Members.Count()));
            })).ToArray());

            var aggregatedCharacters =
                from c in characters
                group c by new { c.AchievementPoints, c.Pets}  into result
                select new AggergatedCharacter
                {
                    Main = result.OrderByDescending(r => r.AverageItemLevelEquipped).First(),
                    Alts = result.OrderByDescending(r => r.AverageItemLevelEquipped).Skip(1).ToList()
                };

            return aggregatedCharacters;
        }

        private Guild GetGuildInternal(string realmName, string guildName)
        {
            try
            {
                return _explorer.GetGuild(realmName, guildName, GuildOptions.GetMembers);
            }
            catch (WebException ex)
            {
                HttpStatusCode code = ((HttpWebResponse)ex.Response).StatusCode;
                if (code == HttpStatusCode.GatewayTimeout)
                {
                    return GetGuildInternal(realmName, guildName);
                }

                throw;
            }
        }

        private Character GetCharacterInternal(GuildMember member)
        {
            try
            {
                return _explorer.GetCharacter(member.Character.Realm, member.Character.Name,
                    CharacterOptions.GetItems | CharacterOptions.GetReputation | CharacterOptions.GetPets);
            }
            catch (WebException ex)
            {
                HttpStatusCode code = ((HttpWebResponse) ex.Response).StatusCode;
                switch (code)
                {
                    case HttpStatusCode.NotFound:
                        return null;
                    case HttpStatusCode.GatewayTimeout:
                        return GetCharacterInternal(member);
                    default:
                        throw;
                }
            }
        }
    }
}
