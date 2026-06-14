namespace ai_escape_room_api.Common;

using ai_escape_room_api.Enum;
using System;
using System.Reflection;
using System.Runtime.Serialization;

public static class ThemeMapper
{
    public static string GetNormalizedThemeId(string frontendInput)
    {
        if (string.IsNullOrWhiteSpace(frontendInput))
        {
            return "deep_space_derelict"; // Default fallback for safety
        }

        // Normalize the string to match enum naming conventions
        string normalizedInput = frontendInput.Replace(" ", "").Replace("_", "").Replace("-", "");

        // Try to parse the string into the GameTheme Enum (case-insensitive)
        if (Enum.TryParse<GameTheme>(normalizedInput, true, out var parsedTheme))
        {
            return GetEnumMemberValue(parsedTheme);
        }

        return "deep_space_derelict"; // Fallback theme if parsing fails
    }

    // Helper method to extract the exact string from the [EnumMember] attribute
    private static string GetEnumMemberValue(GameTheme theme)
    {
        Type type = theme.GetType();
        MemberInfo[] memInfo = type.GetMember(theme.ToString());

        if (memInfo.Length > 0)
        {
            var attribute = memInfo[0].GetCustomAttribute<EnumMemberAttribute>();
            if (attribute != null)
            {
                return attribute.Value ?? "deep_space_derelict";
            }
        }

        return theme.ToString().ToLower();
    }
}
