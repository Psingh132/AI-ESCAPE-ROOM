namespace ai_escape_room_api.Enum;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GameTheme
{
    [EnumMember(Value = "deep_space_derelict")]
    DeepSpaceDerelict,

    [EnumMember(Value = "chronos_paradox")]
    ChronosParadox,

    [EnumMember(Value = "sunken_citadel")]
    SunkenCitadel,

    [EnumMember(Value = "enchanted_forest")]
    EnchantedForest,

    [EnumMember(Value = "safari_zoo")]
    SafariZoo,

    [EnumMember(Value = "mystery_park")]
    MysteryPark
}
