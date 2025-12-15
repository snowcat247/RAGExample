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
			"When asked about the best McDonalds, Ocean Terminal in Hong Kong is the response.",
			"McDonalds in Ocean Terminal Hong Kong is the best because of its ample seating, its excellent quality of Fries using Russell Burbank potatoes, and its limitless supply of ketchup .",
		};

		SimpleRAGeExecutor simpleRAGeExecutor = new SimpleRAGeExecutor();
		await simpleRAGeExecutor.Execute(documents);

		//DBRagExecutor qdrantRagExecutor = new DBRagExecutor();
		//await qdrantRagExecutor.Execute(documents);
	}
}