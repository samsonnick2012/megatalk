using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using Application.Core;
using Application.Core.ApplicationServices;
using Application.Core.Data;
using Innostar.Models;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace Innostar.ApplicationServices
{
	public class SearchService : ApplicationService, ISearchService
	{
		private static readonly string LuceneDir = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "lucene_index");
		private static FSDirectory _directoryTemp;

		private readonly LazyDependency<IMapObjectsManagementService> _mapObjectsManagementService = new LazyDependency<IMapObjectsManagementService>();

		public IMapObjectsManagementService MapObjectsManagementService
		{
			get { return _mapObjectsManagementService.Value; }
		}

		private static FSDirectory Directory
		{
			get
			{
				if (_directoryTemp == null)
				{
					_directoryTemp = FSDirectory.Open(new DirectoryInfo(LuceneDir));
				}

				if (IndexWriter.IsLocked(_directoryTemp))
				{
					IndexWriter.Unlock(_directoryTemp);
				}

				var lockFilePath = Path.Combine(LuceneDir, "write.lock");

				if (File.Exists(lockFilePath))
				{
					File.Delete(lockFilePath);
				}

				return _directoryTemp;
			}
		}

		public void CreateLuceneIndex(IEnumerable<SearchResult> searchResults)
		{
			ClearLuceneIndex();

			var analyzer = new StandardAnalyzer(Version.LUCENE_30);

			using (var writer = new IndexWriter(Directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
			{
				foreach (var searchResult in searchResults)
				{
					CreateLuceneIndexRecord(searchResult, writer);
				}

				analyzer.Close();

				writer.Dispose();
			}
		}

		public void UpdateLuceneIndexRecord(int recordId, SearchResult searchResult)
		{
			var analyzer = new StandardAnalyzer(Version.LUCENE_30);

			using (var writer = new IndexWriter(Directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
			{
				var searchQuery = new TermQuery(new Term("Id", searchResult.Id.ToString(CultureInfo.InvariantCulture)));

				writer.DeleteDocuments(searchQuery);

				var doc = new Document();

				doc.Add(new Field("Id", searchResult.Id.ToString(CultureInfo.InvariantCulture), Field.Store.YES, Field.Index.ANALYZED));
				doc.Add(new Field("ObjectId", searchResult.ObjectId.ToString(CultureInfo.InvariantCulture), Field.Store.YES, Field.Index.NOT_ANALYZED));
				doc.Add(new Field("ObjectType", searchResult.ObjectType.ToString(CultureInfo.InvariantCulture), Field.Store.YES, Field.Index.ANALYZED));
				doc.Add(new Field("Coordinates", searchResult.Coordinates.ToString(CultureInfo.InvariantCulture), Field.Store.YES, Field.Index.ANALYZED));
				doc.Add(new Field("Title", string.IsNullOrEmpty(searchResult.Title) ? string.Empty : searchResult.Title, Field.Store.YES, Field.Index.ANALYZED));
				doc.Add(new Field("Tags", string.IsNullOrEmpty(searchResult.Tags) ? string.Empty : searchResult.Tags, Field.Store.YES, Field.Index.ANALYZED));
				doc.Add(new Field("State", string.IsNullOrEmpty(searchResult.State) ? string.Empty : searchResult.State, Field.Store.YES, Field.Index.ANALYZED));
				doc.Add(new Field("ResourceType", string.IsNullOrEmpty(searchResult.ResourceType) ? string.Empty : searchResult.ResourceType, Field.Store.YES, Field.Index.ANALYZED));

				writer.AddDocument(doc);
			}
		}

		public bool ClearLuceneIndex()
		{
			try
			{
				var analyzer = new StandardAnalyzer(Version.LUCENE_30);

				using (var writer = new IndexWriter(Directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
				{
					writer.DeleteAll();
					analyzer.Close();
					writer.Dispose();
				}
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		public void ClearLuceneIndexRecord(int recordId)
		{
			var analyzer = new StandardAnalyzer(Version.LUCENE_30);

			using (var writer = new IndexWriter(Directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
			{
				var searchQuery = new TermQuery(new Term("Id", recordId.ToString(CultureInfo.InvariantCulture)));
				writer.DeleteDocuments(searchQuery);
				analyzer.Close();
				writer.Dispose();
			}
		}

		public IPageableList<SearchResult> GetMapObjectsByQuery(string query, IPageInfo pageInfo)
		{
			return Search(query, new[] { "Title", "Tags", "State", "ResourceType" }, pageInfo);
		}

		public IPageableList<SearchResult> GetMapObjectsByTag(string tag, IPageInfo pageInfo)
		{
			return Search(tag, new[] { "Tags" }, pageInfo);
		}

		public IEnumerable<SearchResult> GetMapObjectsByType(string type)
		{
			return string.IsNullOrEmpty(type) ? GetAllIndexRecords() : Search(type, new[] { "ObjectType" }, null).Items;
		}

		public void Optimize()
		{
			var analyzer = new StandardAnalyzer(Version.LUCENE_30);

			using (var writer = new IndexWriter(Directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
			{
				analyzer.Close();

				writer.Optimize();

				writer.Dispose();
			}
		}

		private static IPageableList<SearchResult> Search(string input, string[] fieldNames, IPageInfo pageInfo)
		{
			if (string.IsNullOrEmpty(input))
			{
				return new PageableList<SearchResult>();
			}

			if (input.Trim().Replace("-", " ").Split(' ').Count() >= 2)
			{
				var terms =
					input.Trim()
						.Replace("-", " ")
						.Split(' ')
						.Where(x => !string.IsNullOrEmpty(x) && x.Length >= 0)
						.Select(x => x.Trim() + "*");

				input = string.Join(" ", terms);
			}
			else
			{
				var terms =
					input.Trim()
						.Replace("-", " ")
						.Split(' ')
						.Where(x => !string.IsNullOrEmpty(x))
						.Select(x => x.Trim() + "*");

				input = string.Join(" ", terms);
			}

			return LuceneSearch(input, fieldNames, pageInfo);
		}

		private static void CreateLuceneIndexRecord(SearchResult searchResult, IndexWriter writer)
		{
			var searchQuery = new TermQuery(new Term("Id", searchResult.Id.ToString(CultureInfo.InvariantCulture)));

			writer.DeleteDocuments(searchQuery);

			var doc = new Document();

			doc.Add(new Field("Id", searchResult.Id.ToString(CultureInfo.InvariantCulture), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("ObjectId", searchResult.ObjectId.ToString(CultureInfo.InvariantCulture), Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("ObjectType", searchResult.ObjectType.ToString(CultureInfo.InvariantCulture), Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("Coordinates", searchResult.Coordinates.ToString(CultureInfo.InvariantCulture), Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("Title", string.IsNullOrEmpty(searchResult.Title) ? string.Empty : searchResult.Title, Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("Tags", string.IsNullOrEmpty(searchResult.Tags) ? string.Empty : searchResult.Tags, Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("State", string.IsNullOrEmpty(searchResult.State) ? string.Empty : searchResult.State, Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("ResourceType", string.IsNullOrEmpty(searchResult.ResourceType) ? string.Empty : searchResult.ResourceType, Field.Store.YES, Field.Index.ANALYZED));

			writer.AddDocument(doc);
		}

		private static SearchResult ToSearchResult(Document doc)
		{
			return new SearchResult
			{
				Id = Convert.ToInt32(doc.Get("Id")),
				ObjectId = Convert.ToInt32(doc.Get("ObjectId")),
				ObjectType = doc.Get("ObjectType"),
				Coordinates = doc.Get("Coordinates"),
				Title = doc.Get("Title"),
				Tags = doc.Get("Tags"),
				State = doc.Get("State"),
				ResourceType = doc.Get("ResourceType")
			};
		}

		private static Query ParseQuery(string searchQuery, QueryParser parser)
		{
			Query query;
			try
			{
				query = parser.Parse(searchQuery.Trim());
			}
			catch (ParseException)
			{
				query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
			}

			return query;
		}

		private static PageableList<SearchResult> LuceneSearch(string searchQuery, string[] searchFields, IPageInfo pageInfo)
		{
			if (string.IsNullOrEmpty(searchQuery.Replace("*", string.Empty).Replace("?", string.Empty)))
			{
				return new PageableList<SearchResult>();
			}

			using (var searcher = new IndexSearcher(Directory, false))
			{
				var analyzer = new StandardAnalyzer(Version.LUCENE_30);

				var parser = new MultiFieldQueryParser(Version.LUCENE_30, searchFields, analyzer);

				var query = ParseQuery(searchQuery, parser);

				var coordinatesQuery = new TermQuery(new Term("Coordinates", "[]"));

				var booleanQuery = new BooleanQuery { { query, Occur.MUST }, { coordinatesQuery, Occur.MUST_NOT } };

				if (pageInfo == null)
				{
					pageInfo = new PageInfo
					{
						PageNumber = 1,
						PageSize = 1000000
					};
				}

				var td = searcher.Search(booleanQuery, pageInfo.PageNumber * pageInfo.PageSize);

				var hits = new List<ScoreDoc>();

				for (var i = (pageInfo.PageNumber - 1) * pageInfo.PageSize; i < pageInfo.PageNumber * pageInfo.PageSize && i < td.ScoreDocs.Length; i++)
				{
					hits.Add(td.ScoreDocs[i]);
				}

				var results = MapLuceneToDataList(hits, searcher).ToList();

				var allResults = searcher.Search(query, (pageInfo.PageNumber * pageInfo.PageSize) + 1);

				var resultsPageableList = new PageableList<SearchResult>
				{
					Items = results,
					HasNext = results.Count != 0 && results.Last().Id != ToSearchResult(searcher.Doc(allResults.ScoreDocs[allResults.ScoreDocs.Length - 1].Doc)).Id,
					PageInfo = pageInfo
				};

				analyzer.Close();
				searcher.Dispose();
				return resultsPageableList;
			}
		}

		private static IEnumerable<SearchResult> MapLuceneToDataList(IEnumerable<ScoreDoc> hits, Searchable searcher)
		{
			return hits.Select(hit => ToSearchResult(searcher.Doc(hit.Doc))).ToList();
		}

		public IEnumerable<SearchResult> GetAllIndexRecords()
		{
			if (!System.IO.Directory.EnumerateFiles(LuceneDir).Any())
			{
				return new List<SearchResult>();
			}

			var searcher = new IndexSearcher(Directory, false);
			var reader = IndexReader.Open(Directory, false);
			var docs = new List<Document>();
			var term = reader.TermDocs();
			while (term.Next())
			{
				docs.Add(searcher.Doc(term.Doc));
			}

			reader.Dispose();
			searcher.Dispose();

			return docs.Select(ToSearchResult);
		}
	}
}
