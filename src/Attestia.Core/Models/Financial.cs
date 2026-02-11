using System.Text.Json.Serialization;

namespace Attestia.Core.Models;

public sealed record Money
{
    [JsonPropertyName("amount")]
    public required string Amount { get; init; }

    [JsonPropertyName("currency")]
    public required string Currency { get; init; }

    [JsonPropertyName("decimals")]
    public required int Decimals { get; init; }
}

public sealed record AccountRef
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("type")]
    public required AccountType Type { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }
}

[JsonConverter(typeof(JsonStringEnumConverter<AccountType>))]
public enum AccountType
{
    [JsonStringEnumMemberName("asset")] Asset,
    [JsonStringEnumMemberName("liability")] Liability,
    [JsonStringEnumMemberName("income")] Income,
    [JsonStringEnumMemberName("expense")] Expense,
    [JsonStringEnumMemberName("equity")] Equity,
}

[JsonConverter(typeof(JsonStringEnumConverter<LedgerEntryType>))]
public enum LedgerEntryType
{
    [JsonStringEnumMemberName("debit")] Debit,
    [JsonStringEnumMemberName("credit")] Credit,
}

public sealed record LedgerEntry
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("accountId")]
    public required string AccountId { get; init; }

    [JsonPropertyName("type")]
    public required LedgerEntryType Type { get; init; }

    [JsonPropertyName("money")]
    public required Money Money { get; init; }

    [JsonPropertyName("timestamp")]
    public required string Timestamp { get; init; }

    [JsonPropertyName("intentId")]
    public string? IntentId { get; init; }

    [JsonPropertyName("txHash")]
    public string? TxHash { get; init; }

    [JsonPropertyName("correlationId")]
    public required string CorrelationId { get; init; }
}
