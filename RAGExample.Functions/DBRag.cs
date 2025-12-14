
using System.Net.Http;
using System.Text.Json;

namespace RAGExample.Functions
{


	public class DBRag
	{
		private readonly HttpClient _ollamaClient;
		private readonly string _collectionName = "documents";
		private readonly string _ollamaUrl = "http://localhost:11434";

		public DBRag()
		{
			

			_ollamaClient = new HttpClient();
		}

		

		

	}
}
