namespace ai_escape_room_api.Models;

public class UserRequest
{
    // The active chat history session token
    public string? ConversationId { get; set; }
    // The user's input text
    public string TargetAudience { get; set; } = string.Empty;

    public string Theme { get; set; } = string.Empty;

    public string Difficulty { get; set; } = string.Empty;
}