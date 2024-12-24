using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BlockChain;

public class Blockchain
{
    private readonly List<Block> _chain;
    private readonly RSACryptoServiceProvider _rsa;

    public Blockchain()
    {
        _chain = [];
        _rsa = new RSACryptoServiceProvider();
        CreateGenesisBlock();
    }

    private void CreateGenesisBlock()
    {
        var block = new Block
        {
            Index = 0,
            Data = "Genesis Block",
            PreviousHash = "0"
        };
        block.Hash = ComputeHash(block);
        block.HashSignature = SignData(block.Hash);
        _chain.Add(block);
    }

    private static string ComputeHash(Block block)
    {
        var input = $"{block.Index}{block.Data}{block.PreviousHash}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }

    private string SignData(string data)
    {
        var dataBytes = Encoding.UTF8.GetBytes(data);
        return Convert.ToBase64String(_rsa.SignData(dataBytes,
            CryptoConfig.MapNameToOID(HashAlgorithmName.SHA256.Name!)!));
    }

    private bool VerifySignature(string data, string signature)
    {
        var dataBytes = Encoding.UTF8.GetBytes(data);
        var signatureBytes = Convert.FromBase64String(signature);
        return _rsa.VerifyData(dataBytes, CryptoConfig.MapNameToOID(HashAlgorithmName.SHA256.Name!)!, signatureBytes);
    }

    public void AddBlock(string data)
    {
        var lastBlock = _chain[^1];
        var newBlock = new Block
        {
            Index = lastBlock.Index + 1,
            Data = data,
            PreviousHash = lastBlock.Hash
        };
        newBlock.Hash = ComputeHash(newBlock);
        newBlock.HashSignature = SignData(newBlock.Hash);
        newBlock.DataSignature = SignData(data);
        _chain.Add(newBlock);
    }

    public void DisplayBlock(int index)
    {
        if (index < 0 || index >= _chain.Count)
        {
            Console.WriteLine("Invalid block index.");
            return;
        }

        var block = _chain[index];
        Console.WriteLine(JsonSerializer.Serialize(block, new JsonSerializerOptions { WriteIndented = true }));
    }

    public bool VerifyBlock(int index)
    {
        if (index < 0 || index >= _chain.Count) return false;

        var block = _chain[index];
        var isHashValid = VerifySignature(block.Hash, block.HashSignature);
        var isDataValid = VerifySignature(block.Data, block.DataSignature);
        return isHashValid && isDataValid;
    }

    public bool VerifyChain()
    {
        for (var i = 1; i < _chain.Count; i++)
        {
            var current = _chain[i];
            var previous = _chain[i - 1];

            if (current.PreviousHash != previous.Hash || !VerifySignature(current.Hash, current.HashSignature))
                return false;
        }

        return true;
    }
}