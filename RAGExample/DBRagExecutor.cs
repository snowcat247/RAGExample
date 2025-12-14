using RAGExample.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAGExample.Executor
{

	//this is apparently a much more advanced way to query. It can use vector dbs , e.g. document dbs etc.
	public class DBRagExecutor
	{
		public DBRagExecutor() { }
		public async Task Execute(string[] documents)
		{
			DBRag rag = new DBRag();
			//rag.InitializeAsync().Wait();
			//for(int i = 0; i < documents.Length; i++)
			//{
			//	await rag.AddDocumentAsync(i.ToString(), documents[i]);
			//}

			//while (true)
			//{
			//	Console.Write("\nAsk a question (or 'quit'): ");
			//	var question = Console.ReadLine();

			//	if (question?.ToLower() == "quit")
			//		break;

			//	Console.WriteLine("Thinking...");
			//	var answer = await rag.QueryAsync(question);
			//	Console.WriteLine($"Answer: {answer}");
			//}

		}

		

		

	}
}
