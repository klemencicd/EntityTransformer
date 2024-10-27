using System.Text.Json;

namespace EntityTransformerApi.Plicies;

public class LowercaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToLower();
}
