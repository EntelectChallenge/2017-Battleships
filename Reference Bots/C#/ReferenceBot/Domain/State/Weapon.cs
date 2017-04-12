using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ReferenceBot.Domain.State
{
    public class Weapon
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public WeaponType WeaponType;
    }

    public enum WeaponType
    {
        SingleShot
    }
}
