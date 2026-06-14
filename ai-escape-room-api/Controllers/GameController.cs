using ai_escape_room_api.Common;
using ai_escape_room_api.Models;
using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ai_escape_room_api.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly AIProjectClient _projectClient;
    private readonly ProjectResponsesClient _responseClient;
    private readonly ILogger<GameController> _logger;

    public GameController(
        IConfiguration configuration,
        ILogger<GameController> logger)
    {
        _logger = logger;

        // Retrieve configuration strings
        string endpoint = configuration["AzureAI:Endpoint"]
            ?? "https://priyadarshi-9559"; // Fallback to provided default endpoint

        string agentName = configuration["AzureAI:AgentName"]
            ?? "ai-room";

        string agentVersion = configuration["AzureAI:AgentVersion"]
            ?? "2";

        // Initialize the unified project client
        _projectClient = new AIProjectClient(
            endpoint: new Uri(endpoint),
            tokenProvider: new DefaultAzureCredential());

        // Create the reference pointing to your specific Foundry Agent 
        AgentReference agentReference = new(name: agentName, version: agentVersion);

        // Fetch the corresponding Response client for running operations
        _responseClient = _projectClient.ProjectOpenAIClient.GetProjectResponsesClientForAgent(agentReference);
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartGame([FromBody] UserRequest request)
    {
        try
        {
            string cleanThemeId = ThemeMapper.GetNormalizedThemeId(request.Theme);
            // Build the generation query payload
            // All your heavy prompt logic is already saved in the Foundry Portal!
            // Update your backend prompt to look exactly like this:
            // NOTICE THE THREE '$' CHARACTERS HERE
            var userPrompt = $"""
                You are an escape room generator. Generate an escape room for the following configuration and return the result as a JSON object.
                
                Audience: {request.TargetAudience}
                Theme: {cleanThemeId}
                Difficulty: {request.Difficulty}
                
                The JSON object must have exactly these fields at the top level:
                - roomName: a creative title for the room
                - introduction: an immersive opening paragraph
                - difficulty: "{request.Difficulty}"
                - theme: "{cleanThemeId}"
                - puzzles: an array of exactly 3 puzzle objects
                
                Each puzzle object must have exactly these fields:
                - sequenceNumber: 1, 2, or 3
                - puzzleTitle: a short title
                - description: the story narrative followed by the complete puzzle text
                - cluesProvided: an array of 2 hint strings
                - expectedSolution: the exact answer string
                - unlocksText: what the player discovers after solving this puzzle
                
                CRITICAL RULES:
                - You must ALWAYS search the knowledge base before answering any question
                - Every puzzle must have exactly one unambiguous correct answer
                - The clue logic inside the description must directly and uniquely produce the expectedSolution
                - Do not generate puzzles where the answer depends on real-world knowledge not stated in the puzzle itself
                - Do not generate sequence or ordering puzzles unless the ordering rule produces a unique result that is explicitly stated and verifiable within the puzzle text
                - If the puzzle says "order by X", verify that applying rule X to the given items produces exactly one possible sequence before using it
                
                Return only the JSON object. Do not include any explanation or markdown formatting.
                """;

            // CreateResponse performs the underlying synchronous execution round-trip
            // wrap in Task.Run to keep your API controller asynchronous and responsive
            var responseResult = await Task.Run(() => _responseClient.CreateResponse(userPrompt, request.ConversationId));

            // Extract the generated message payload text
            string aiResponse = responseResult.Value.GetOutputText();

            if (string.IsNullOrWhiteSpace(aiResponse))
            {
                return StatusCode(500, new { error = "Agent returned an empty response layout." });
            }

            // Scrub any unwanted markdown syntax artifacts block code fences
            aiResponse = CleanJsonFormatting(aiResponse);

            // Deserialize the response into our strong-typed DTO configuration
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var gameData = JsonSerializer.Deserialize<EscapeRoomResponse>(aiResponse, options);
            if (gameData == null || string.IsNullOrWhiteSpace(gameData.RoomName) || gameData.Puzzles.Count == 0)
            {
                return StatusCode(500, new
                {
                    error = "Agent returned an unexpected response schema.",
                    raw = aiResponse
                });
            }

            return Ok(gameData);
        }
        catch (JsonException jsonEx)
        {
            _logger.LogError(jsonEx, "Failed parsing structural JSON return value from agent.");
            return StatusCode(500, new { error = "Internal schema formatting error. AI failed structural alignment rules." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during escape room execution runtime loop.");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Guard helper to strip out potential markdown block wrappers (```json ... ```) 
    /// if the AI outputs code fences despite system instruction constraints.
    /// </summary>
    private static string CleanJsonFormatting(string rawText)
    {
        string trimmed = rawText.Trim();
        if (trimmed.StartsWith("```json"))
        {
            trimmed = trimmed[7..];
        }
        else if (trimmed.StartsWith("```"))
        {
            trimmed = trimmed[3..];
        }

        if (trimmed.EndsWith("```"))
        {
            trimmed = trimmed[..^3];
        }

        return trimmed.Trim();
    }
}