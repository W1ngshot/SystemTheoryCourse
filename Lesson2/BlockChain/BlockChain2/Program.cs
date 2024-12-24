using BlockChain2;

var blockchain = new Blockchain();

await blockchain.CreateGenesisBlockAsync("Генезис-блок");

await blockchain.AddBlockAsync("Блок 1: транзакция A -> B");
await blockchain.AddBlockAsync("Блок 2: транзакция C -> D");

for (var i = 0; i < blockchain.Chain.Count; i++)
{
    Console.WriteLine(blockchain.GetBlockInfo(i));
}

Console.WriteLine(blockchain.ValidateBlockchain()
    ? "Цепочка блоков валидна."
    : "Цепочка блоков повреждена.");