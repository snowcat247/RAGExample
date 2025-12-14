namespace RAGExample.Functions
{
	public class DocumentRagSystem
	{
		private readonly SimpleRag _rag;

		public DocumentRagSystem()
		{
			_rag = new SimpleRag();
		}

		public void ProcessDocument(string document, int chunkSize = 500)
		{
			// Simple chunking by sentences
			var sentences = document.Split('.', '!', '?');
			var chunks = new List<string>();
			var currentChunk = "";

			foreach (var sentence in sentences)
			{
				if ((currentChunk + sentence).Length > chunkSize)
				{
					if (!string.IsNullOrWhiteSpace(currentChunk))
						chunks.Add(currentChunk.Trim());
					currentChunk = sentence;
				}
				else
				{
					currentChunk += sentence + ". ";
				}
			}

			if (!string.IsNullOrWhiteSpace(currentChunk))
				chunks.Add(currentChunk.Trim());

			foreach (var chunk in chunks)
			{
				_rag.AddDocument(chunk);
			}
		}

		public async Task<string> AskQuestionAsync(string question)
		{
			return await _rag.QueryAsync(question);
		}
	}
}
