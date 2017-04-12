using Newtonsoft.Json;

namespace Domain.Maps
{
    public interface ICellOccupant
    {
        [JsonProperty]
        Cell Cell { get; set; }
        bool Damage();
    }
}