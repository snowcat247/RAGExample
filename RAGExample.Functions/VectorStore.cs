using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAGExample.Functions
{
	public class VectorStore
	{
		private readonly LiteDatabase _db;
		private readonly ILiteCollection<VectorDocument> _vectors;

		public VectorStore(string dbPath = "vectors.db")
		{
			_db = new LiteDatabase(dbPath);
			_vectors = _db.GetCollection<VectorDocument>("vectors");
			_vectors.EnsureIndex(x => x.Id);
		}

		public void Upsert(string id, float[] vector, string text)
		{
			var doc = new VectorDocument
			{
				Id = id,
				Text = text,
				Vector = vector,
				CreatedAt = DateTime.UtcNow
			};

			_vectors.Upsert(doc);
		}

		public List<SearchResult> Search(float[] queryVector, int limit = 5)
		{
			return _vectors.FindAll()
				.Select(doc => new SearchResult
				{
					Text = doc.Text,
					Score = CosineSimilarity(queryVector, doc.Vector)
				})
				.OrderByDescending(x => x.Score)
				.Take(limit)
				.ToList();
		}

		private float CosineSimilarity(float[] a, float[] b)
		{
			float dot = 0, normA = 0, normB = 0;
			for (int i = 0; i < a.Length; i++)
			{
				dot += a[i] * b[i];
				normA += a[i] * a[i];
				normB += b[i] * b[i];
			}
			return dot / (MathF.Sqrt(normA) * MathF.Sqrt(normB));
		}
	}

	public class VectorDocument
	{
		public string Id { get; set; }
		public string Text { get; set; }
		public float[] Vector { get; set; }
		public DateTime CreatedAt { get; set; }
	}

	public class SearchResult
	{
		public string Text { get; set; }
		public float Score { get; set; }
	}
}
