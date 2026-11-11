using System.Text.Json.Serialization;

namespace DatabasePlugin
{
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(UserInput))]
    [JsonSerializable(typeof(HistoryInput))]
    [JsonSerializable(typeof(UserView))]
    [JsonSerializable(typeof(UserView[]))]
    [JsonSerializable(typeof(RoleView))]
    [JsonSerializable(typeof(RoleView[]))]
    [JsonSerializable(typeof(HistoryView))]
    [JsonSerializable(typeof(HistoryView[]))]
    internal partial class DatabaseJsonContext : JsonSerializerContext
    {
    }
}
