using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RAGExample.Functions
{
	public class SimpleRag
	{
		private readonly HttpClient _httpClient;
		private readonly string _ollamaUrl = "http://localhost:11434";
		private List<string> _documents = new List<string>();
		private Dictionary<string, float[]> _embeddings = new Dictionary<string, float[]>();

		public SimpleRag()
		{
			_httpClient = new HttpClient();
		}

		// First, we load the documents that we are using to provide context for our RAG system to memory
		public void AddDocument(string text)
		{
			_documents.Add(text);
		}

		// Next, we use the embeddings api from Ollama to generate embeddings
		public async Task GenerateEmbeddingsAsync()
		{
			foreach (var doc in _documents)
			{
				var embedding = await GetEmbeddingAsync(doc);
				_embeddings[doc] = embedding;
			}
		}

		private async Task<float[]> GetEmbeddingAsync(string text)
		{
			var request = new
			{
				model = "nomic-embed-text", // this was the embedding model I added to Ollama
				prompt = text
			};

			var json = JsonSerializer.Serialize(request);
			var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

			var response = await _httpClient.PostAsync($"{_ollamaUrl}/api/embeddings", content);
			var responseJson = await response.Content.ReadAsStringAsync();

			var doc = JsonDocument.Parse(responseJson);
			return doc.RootElement.GetProperty("embedding")
				.EnumerateArray()
				.Select(e => (float)e.GetDouble())
				.ToArray();
		}

		// This is pretty cool. to identify semantically similar documents, we can use cosine similarity between the query embedding and each document embedding
		private List<string> SearchRelevantDocuments(string query, int topK = 3)
		{
			var queryEmbedding = GetEmbeddingAsync(query).Result;

			var similarities = _embeddings
				.Select(kv => new
				{
					Document = kv.Key,
					Similarity = CosineSimilarity(queryEmbedding, kv.Value)
				})
				.OrderByDescending(x => x.Similarity)
				.Take(topK)
				.Select(x => x.Document)
				.ToList();

			return similarities;
		}

		// Cosine similarity. We calculate the angle of similarity through the dot product of the two vectors divided by the product of their magnitudes

		private float CosineSimilarity(float[] vec1, float[] vec2)
		{
			float dot = 0, norm1 = 0, norm2 = 0;
			for (int i = 0; i < vec1.Length; i++)
			{
				dot += vec1[i] * vec2[i];
				norm1 += vec1[i] * vec1[i];
				norm2 += vec2[i] * vec2[i];
			}
			return dot / (float)(Math.Sqrt(norm1) * Math.Sqrt(norm2));
		}

	
		public async Task<string> QueryAsync(string question)
		{
			// First we search our "database" for doucments that are relevant to the question
			var relevantDocs = SearchRelevantDocuments(question);

			// We use this context to provide additional information to the model
			var context = string.Join("\n\n", relevantDocs);

			// Query Ollama with context
			var prompt = $"""
        Use the following context to answer the question. 
        If you don't know the answer based on the context, say so.
        
        Context:
        {context}
        
        Question: {question}
        
        Answer:
        """;

			return await QueryOllamaAsync(prompt);
		}

		private async Task<string> QueryOllamaAsync(string prompt)
		{
			var request = new
			{
				model = "testmodel", // my local Model name
				prompt = prompt,
				stream = false
			};

			var json = JsonSerializer.Serialize(request);
			var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

			var response = await _httpClient.PostAsync($"{_ollamaUrl}/api/generate", content);
			var responseJson = await response.Content.ReadAsStringAsync();

			var doc = JsonDocument.Parse(responseJson);
			return doc.RootElement.GetProperty("response").GetString();
			
		}
	}
}
