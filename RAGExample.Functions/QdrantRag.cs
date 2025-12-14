using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.Text.Json;

namespace RAGExample.Functions
{


	public class QdrantRag
	{
		private readonly QdrantClient _qdrantClient;
		private readonly HttpClient _ollimaClient;
		private readonly string _collectionName = "documents";

		public QdrantRag()
		{
			_qdrantClient = new QdrantClient("localhost", 6333);
			_ollimaClient = new HttpClient();
		}

		public async Task InitializeAsync()
		{
			// Create collection if it doesn't exist
			var collections = await _qdrantClient.ListCollectionsAsync();
			if (!collections.Contains(_collectionName))
			{
				await _qdrantClient.CreateCollectionAsync(
					_collectionName,
					new VectorParams { Size = 768, Distance = Distance.Cosine }
				);
			}
		}

		public async Task AddDocumentAsync(string id, string text)
		{
			var embedding = await GetEmbeddingAsync(text);

			await _qdrantClient.UpsertAsync(
				_collectionName,
				new[] { new PointStruct { Id = new PointId { Uuid = id }, Vectors = embedding, Payload = { ["text"] = text } } }
			);
		}

		public async Task<List<string>> SearchAsync(string query, int limit = 3)
		{
			var queryEmbedding = await GetEmbeddingAsync(query);

			var results = await _qdrantClient.SearchAsync(
				_collectionName,
				queryEmbedding,
				limit: (ulong)limit
			);

			return results.Select(r => r.Payload["text"].StringValue).ToList();
		}

		private async Task<float[]> GetEmbeddingAsync(string text)
		{
			var request = new
			{
				model = "nomic-embed-text",
				prompt = text
			};

			var json = JsonSerializer.Serialize(request);
			var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

			var response = await _ollimaClient.PostAsync(
				"http://localhost:11434/api/embeddings",
				content
			);

			var responseJson = await response.Content.ReadAsStringAsync();
			using var doc = JsonDocument.Parse(responseJson);

			return doc.RootElement.GetProperty("embedding")
				.EnumerateArray()
				.Select(e => (float)e.GetDouble())
				.ToArray();
		}
	}
}
