using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KomugikoBot
{
    public sealed class Komugiko
    {
        public DateTime StartDateUTC { get; private set; }

        private static Komugiko? instance = null;
        public static Komugiko Instance
        {
            get
            {
                if (instance == null)
                    instance = new Komugiko();
                return instance;
            }
        }

        private Komugiko()
        {
            Console.WriteLine("Bot Started: " + DateTime.Now.ToUniversalTime());
            StartDateUTC = DateTime.Now.ToUniversalTime();
        }

        public TimeSpan CurrentUptimeUTC => DateTime.Now.ToUniversalTime() - StartDateUTC; //00:00:17.7278547

        public static Dictionary<ulong, FormSettingsInfo> deployedforms = new Dictionary<ulong, FormSettingsInfo>();

        public struct FormSettingsInfo
        {
            public readonly bool IsNormalized;
            public readonly HashSet<IEmote> registeredEmojis;

            public FormSettingsInfo(bool isNormalized, HashSet<IEmote> registeredEmojis)
            {
                IsNormalized = isNormalized;
                this.registeredEmojis = registeredEmojis;
            }
        }
    }
}