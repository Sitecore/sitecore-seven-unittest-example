namespace ContentSearchTest
{
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using FakeItEasy;

    using FluentAssertions;

    using Sitecore.ContentSearch;
    using Sitecore.ContentSearch.Linq;
    using Sitecore.ContentSearch.Security;

    [TestFixture]
    public class ExampleFixture
    {
        private ISearchIndex fakeIndex;
        private IProviderSearchContext fakeSearchContext;

        #region A region here simply because Martin hates regions
        #endregion

        [SetUp]
        public void Setup()
        {
            fakeIndex = A.Fake<ISearchIndex>();
            fakeSearchContext = A.Fake<IProviderSearchContext>();
            
            A.CallTo(() => fakeIndex.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck)).Returns(fakeSearchContext);
        }

        #region Simple Test - Fake all results + queryable

        [Test]
        public void Queryable_CanQueryFakeRepo_IssuesLinqCorrectly()
        {
            // Arrange

            var listDoc = new List<TestDocument>
                              {
                                  new TestDocument { Id = 1, Title = "monkey" },
                                  new TestDocument { Id = 2, Title = "cat" },
                                  new TestDocument { Id = 3, Title = "cat" }
                              };

            A.CallTo(() => fakeSearchContext.GetQueryable<TestDocument>()).Returns(new SimpleFakeRepo<TestDocument>(listDoc));

            // Act
            var ctx = fakeIndex.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck);
            var queryable = ctx.GetQueryable<TestDocument>();
            var results = queryable.Where(t => t.Title.Contains("cat")).ToList();

            // Assert
            ctx.Should().NotBeNull();
            queryable.Should().NotBeNull();
            results.Count.Should().Be(2);
        }

        #endregion

        #region Advanced Test - Testing with custom extension methods

        [Test]
        public void Queryable_EnhancedResults_ReturnCorrectAmountofResults()
        {
            // Arrange
            var listDoc = new List<TestDocument>
                              {
                                  new TestDocument { Id = 1, Title = "monkey" },
                                  new TestDocument { Id = 2, Title = "cat" },
                                  new TestDocument { Id = 3, Title = "cat" }
                              };

            var fakeQueryable = new AdvancedFakeRepo<TestDocument>(listDoc);
            A.CallTo(() => fakeSearchContext.GetQueryable<TestDocument>()).Returns(fakeQueryable);

            //Act
            var ctx = fakeIndex.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck);
            var queryable = ctx.GetQueryable<TestDocument>();
            var query = queryable.Where(t => t.Title.Contains("cat"));
            var results = fakeQueryable.GetResults(query);

            // Assert
            results.Should().NotBeNull();
            results.TotalSearchResults.Should().Be(2);
        }

        #endregion
    }

    #region Example POCO

    public class TestDocument
    {
        [IndexField("_id")]
        public int Id { get; set; }

        public string Title { get; set; }

        [IndexField("_templatename")]
        public string TemplateName { get; set; }
    }

    #endregion

    #region IQueryable - Simple enumerable result store

    public class SimpleFakeRepo<T> : EnumerableQuery<T>
    {
        public SimpleFakeRepo(IEnumerable<T> enumerable): base(enumerable)
        {
        }
    }

    #endregion

    #region IQueryable - Advanced result store

    public class AdvancedFakeRepo<T> : EnumerableQuery<T>, IEnhanceResults<T>
    {
      public AdvancedFakeRepo(IEnumerable<T> enumerable): base(enumerable)
      {
      }

      public SearchResults<T> GetResults(IQueryable<T> queryable)
      {
        return new SearchResults<T>(queryable.Select(document => new SearchHit<T>(1.0f, document)), queryable.Count());
      }

      public FacetResults GetFacets(IQueryable<T> queryable)
      {
        return new FacetResults();
      }
    }

    public interface IEnhanceResults<T>
    {
        SearchResults<T> GetResults(IQueryable<T> queryable);

        FacetResults GetFacets(IQueryable<T> queryable);
    }

    #endregion

}