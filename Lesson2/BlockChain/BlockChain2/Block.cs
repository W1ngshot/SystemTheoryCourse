using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlockChain2;

public class Block(string data, string previousHash = "")
{
    public string PreviousHash { get; set; } = previousHash;
    public string Data { get; set; } = data;
    public string? Signature { get; set; }
    public string? Hash { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    private const string TimestampServiceUrl = "http://itislabs.ru/ts?digest=";

    public async Task FinalizeBlockAsync()
    {
        if (string.IsNullOrEmpty(PreviousHash))
            PreviousHash = "0";

        var hashBytes = Encoding.UTF8.GetBytes(PreviousHash + Data);
        var hash = Convert.ToHexString(SHA256.HashData(hashBytes));
        
        await SignBlockWithTimestampAsync(hash);
    }

    private async Task SignBlockWithTimestampAsync(string hash)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(TimestampServiceUrl + hash);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Ошибка при запросе к API: {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Ответ API: " + jsonResponse);

        var timestampResponse = JsonSerializer.Deserialize<TimeStampResponse>(jsonResponse);

        if (timestampResponse == null)
        {
            throw new Exception("Ошибка: не удалось десериализовать ответ API.");
        }

        Console.WriteLine($"Статус: {timestampResponse.Status}");
        Console.WriteLine($"Строка статуса: {timestampResponse.StatusString}");


        if (timestampResponse.TimeStampToken == null ||
            string.IsNullOrWhiteSpace(timestampResponse.TimeStampToken.Ts) ||
            string.IsNullOrWhiteSpace(timestampResponse.TimeStampToken.Signature))
        {
            throw new Exception("Метка времени или подпись не были получены из ответа API.");
        }

        var ts = timestampResponse.TimeStampToken.Ts;
        var signature = timestampResponse.TimeStampToken.Signature;

        Console.WriteLine($"Метка времени (ts): {ts}");
        Console.WriteLine($"Подпись: {signature}");

        Timestamp = DateTime.Parse(ts);
        Signature = signature;
        var hashBytes = Encoding.UTF8.GetBytes(PreviousHash + Data + Signature);
        Hash = Convert.ToHexString(SHA256.HashData(hashBytes));
    }

    public class TimeStampResponse
    {
        public int Status { get; set; }
        public string StatusString { get; set; }

        [JsonPropertyName("timeStampToken")] public TimeStampToken TimeStampToken { get; set; }
    }

    public class TimeStampToken
    {
        [JsonPropertyName("ts")] public string Ts { get; set; }

        [JsonPropertyName("signature")] public string? Signature { get; set; }
    }
}