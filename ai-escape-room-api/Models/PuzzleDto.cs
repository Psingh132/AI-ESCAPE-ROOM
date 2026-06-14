using System.Text.Json.Serialization;

namespace ai_escape_room_api.Models;

public class PuzzleDto
{
    [JsonPropertyName("sequenceNumber")]
    public int SequenceNumber { get; set; }

    [JsonPropertyName("puzzleTitle")]
    public string PuzzleTitle { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("cluesProvided")]
    public List<string> CluesProvided { get; set; } = new();

    [JsonPropertyName("expectedSolution")]
    public string ExpectedSolution { get; set; } = string.Empty;

    [JsonPropertyName("unlocksText")]
    public string UnlocksText { get; set; } = string.Empty;
}
