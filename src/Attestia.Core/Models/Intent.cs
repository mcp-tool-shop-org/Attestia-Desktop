using System.Text.Json.Serialization;

namespace Attestia.Core.Models;

[JsonConverter(typeof(JsonStringEnumConverter<IntentStatus>))]
public enum IntentStatus
{
    [JsonStringEnumMemberName("declared")] Declared,
    [JsonStringEnumMemberName("approved")] Approved,
    [JsonStringEnumMemberName("rejected")] Rejected,
    [JsonStringEnumMemberName("executing")] Executing,
    [JsonStringEnumMemberName("executed")] Executed,
    [JsonStringEnumMemberName("verified")] Verified,
    [JsonStringEnumMemberName("failed")] Failed,
}

public sealed record Intent
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("status")]
    public required IntentStatus Status { get; init; }

    [JsonPropertyName("kind")]
    public required string Kind { get; init; }

    [JsonPropertyName("description")]
    public required string Description { get; init; }

    [JsonPropertyName("declaredBy")]
    public required string DeclaredBy { get; init; }

    [JsonPropertyName("declaredAt")]
    public required string DeclaredAt { get; init; }

    [JsonPropertyName("params")]
    public required Dictionary<string, object?> Params { get; init; }
}

public sealed record IntentDeclaration
{
    [JsonPropertyName("intentId")]
    public required string IntentId { get; init; }

    [JsonPropertyName("declaredBy")]
    public required string DeclaredBy { get; init; }

    [JsonPropertyName("declaredAt")]
    public required string DeclaredAt { get; init; }

    [JsonPropertyName("kind")]
    public required string Kind { get; init; }

    [JsonPropertyName("params")]
    public required Dictionary<string, object?> Params { get; init; }
}

public sealed record IntentApproval
{
    [JsonPropertyName("intentId")]
    public required string IntentId { get; init; }

    [JsonPropertyName("approvedBy")]
    public required string ApprovedBy { get; init; }

    [JsonPropertyName("approvedAt")]
    public required string ApprovedAt { get; init; }

    [JsonPropertyName("approved")]
    public required bool Approved { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }
}

public sealed record IntentExecution
{
    [JsonPropertyName("intentId")]
    public required string IntentId { get; init; }

    [JsonPropertyName("executedAt")]
    public required string ExecutedAt { get; init; }

    [JsonPropertyName("chainId")]
    public required string ChainId { get; init; }

    [JsonPropertyName("txHash")]
    public required string TxHash { get; init; }
}

public sealed record IntentVerification
{
    [JsonPropertyName("intentId")]
    public required string IntentId { get; init; }

    [JsonPropertyName("verifiedAt")]
    public required string VerifiedAt { get; init; }

    [JsonPropertyName("matched")]
    public required bool Matched { get; init; }

    [JsonPropertyName("discrepancies")]
    public IReadOnlyList<string>? Discrepancies { get; init; }
}
