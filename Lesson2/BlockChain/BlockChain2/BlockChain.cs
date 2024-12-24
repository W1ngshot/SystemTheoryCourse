using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BlockChain2;

public class Blockchain
{
    public List<Block> Chain { get; private set; }
    private const string BlockchainFile = "blockchain.json";

    public Blockchain()
    {
        Chain = LoadBlockchainFromFile();
    }
    
    public async Task CreateGenesisBlockAsync(string data)
    {
        if (Chain.Count == 0)
        {
            var genesisBlock = new Block(data);
            await genesisBlock.FinalizeBlockAsync();
            Chain.Add(genesisBlock);
            SaveBlockchainToFile();
        }
    }
    
    public async Task AddBlockAsync(string data)
    {
        if (Chain.Count == 0)
        {
            throw new InvalidOperationException("Генезис-блок отсутствует. Сначала создайте его.");
        }

        var previousBlock = Chain[^1];
        var newBlock = new Block(data, previousBlock.Hash!);
        await newBlock.FinalizeBlockAsync();
        Chain.Add(newBlock);

        SaveBlockchainToFile();
    }
    
    public bool ValidateBlockchain()
    {
        for (var i = 1; i < Chain.Count; i++)
        {
            var currentBlock = Chain[i];
            var previousBlock = Chain[i - 1];

            if (currentBlock.PreviousHash != previousBlock.Hash)
                return false;

            var recomputedHash = ComputeHash(currentBlock.PreviousHash, currentBlock.Data, currentBlock.Signature);
            if (currentBlock.Hash != recomputedHash)
                return false;
        }

        return true;
    }
    
    private void SaveBlockchainToFile()
    {
        var json = JsonSerializer.Serialize(Chain, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(BlockchainFile, json);
    }
    
    private List<Block> LoadBlockchainFromFile()
    {
        if (!File.Exists(BlockchainFile))
            return [];

        var json = File.ReadAllText(BlockchainFile);
        return JsonSerializer.Deserialize<List<Block>>(json)!;
    }
    
    private string ComputeHash(string previousHash, string data, string? signature)
    {
        var hashBytes = Encoding.UTF8.GetBytes(previousHash + data + signature);
        return Convert.ToHexString(SHA256.HashData(hashBytes));
    }
    
    public string GetBlockInfo(int index)
    {
        if (index < 0 || index >= Chain.Count)
        {
            return "Блок с указанным индексом отсутствует.";
        }

        var block = Chain[index];
        return $@"
Индекс: {index}
Данные: {block.Data}
Хеш: {block.Hash}
Хеш предыдущего блока: {block.PreviousHash}
Метка времени: {block.Timestamp}
Подпись: {block.Signature}";
    }
}