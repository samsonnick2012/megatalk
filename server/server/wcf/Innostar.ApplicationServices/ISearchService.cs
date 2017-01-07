using System.Collections.Generic;
using Application.Core.ApplicationServices;
using Application.Core.Data;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
    public interface ISearchService : IApplicationService
    {
        void CreateLuceneIndex(IEnumerable<SearchResult> searchResults);

        void UpdateLuceneIndexRecord(int recordId, SearchResult searchResult);

        void ClearLuceneIndexRecord(int recordId);

        bool ClearLuceneIndex();

        void Optimize();

		IPageableList<SearchResult> GetMapObjectsByTag(string tag, IPageInfo pageInfo);

		IPageableList<SearchResult> GetMapObjectsByQuery(string query, IPageInfo pageInfo);

		IEnumerable<SearchResult> GetMapObjectsByType(string type);

	    IEnumerable<SearchResult> GetAllIndexRecords();
    }
}
