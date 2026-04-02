using System.Text.Json.Serialization;

namespace ApiGateway.DTOs
{
    public class AuthRegisterResponse
    {
        [JsonPropertyName("id")]
        public Guid UserId { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public int Role { get; set; }
    }
}