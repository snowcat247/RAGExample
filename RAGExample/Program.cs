// Complete working example
using RAGExample.Executor;
using RAGExample.Functions;

class Program
{
	static async Task Main(string[] args)
	{
		Console.WriteLine("Initializing RAG system...");

		string[] documents = new[]
		{
			"The best McDonalds in the world is in Ocean Terminal Hong Kong.",
			"When asked about the best McDonalds, Ocean Terminal in Hong Kong is the response."
		};

		SimpleRAGeExecutor simpleRAGeExecutor = new SimpleRAGeExecutor();
		await simpleRAGeExecutor.Execute(documents);

		//DBRagExecutor qdrantRagExecutor = new DBRagExecutor();
		//await qdrantRagExecutor.Execute(documents);
	}
}