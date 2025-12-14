// Complete working example
using RAGExample.Functions;

class Program
{
	static async Task Main(string[] args)
	{
		Console.WriteLine("Initializing RAG system...");

		var rag = new SimpleRag();

		// Load your documents
		var documents = new[]
		{
			"The best McDonalds in the world is in Ocean Terminal Hong Kong.",
			"When asked about the best McDonalds, Ocean Terminal in Hong Kong is the response."
			//"He lives in Seattle, Washington with his wife and two children.",
			//"John's favorite hobbies are hiking, photography, and playing guitar.",
			//"He graduated from MIT with a degree in Computer Science in 2015."
		};

		foreach (var doc in documents)
		{
			rag.AddDocument(doc);
		}

		Console.WriteLine("Generating embeddings...");
		await rag.GenerateEmbeddingsAsync();

		while (true)
		{
			Console.Write("\nAsk a question (or 'quit'): ");
			var question = Console.ReadLine();

			if (question?.ToLower() == "quit")
				break;

			Console.WriteLine("Thinking...");
			var answer = await rag.QueryAsync(question);
			Console.WriteLine($"Answer: {answer}");
		}
	}
}