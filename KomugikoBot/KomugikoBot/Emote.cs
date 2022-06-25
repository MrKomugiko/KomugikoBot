using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KomugikoBot
{
    internal class Emote : IEmote
    {
        private string _name;
        public Emote(string name)
        {
            _name = name.Trim().TrimEnd().TrimStart();   
        }

        public string Name => _name;
    }
}
