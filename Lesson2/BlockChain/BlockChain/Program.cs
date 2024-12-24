using BlockChain;

var blockchain = new Blockchain();

blockchain.AddBlock("Block 1 Data");
blockchain.AddBlock("Block 2 Data");

blockchain.DisplayBlock(1);

Console.WriteLine($"Block 1 Valid: {blockchain.VerifyBlock(1)}");
Console.WriteLine($"Chain Valid: {blockchain.VerifyChain()}");