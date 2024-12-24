namespace BlockChain;

public class Block
{
    public int Index { get; set; }
    public string Data { get; set; }
    public string Hash { get; set; }
    public string PreviousHash { get; set; }
    public string DataSignature { get; set; }
    public string HashSignature { get; set; }
}