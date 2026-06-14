using System.Text.Json.Serialization;

namespace ai_escape_room_api.Models;

public class EscapeRoomResponse
{
    [JsonPropertyName("roomName")]
    public string RoomName { get; set; } = string.Empty;

    [JsonPropertyName("introduction")]
    public string Introduction { get; set; } = string.Empty;

    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; } = string.Empty;

    [JsonPropertyName("theme")]
    public string Theme { get; set; } = string.Empty;

    [JsonPropertyName("puzzles")]
    public List<PuzzleDto> Puzzles { get; set; } = new();
}
