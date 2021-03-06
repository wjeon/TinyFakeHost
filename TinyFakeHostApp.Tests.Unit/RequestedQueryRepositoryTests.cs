﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TinyFakeHostApp.Persistence;
using TinyFakeHostApp.RequestResponse;

namespace TinyFakeHostApp.Tests.Unit
{
    [TestFixture]
    public class RequestedQueryRepositoryTests
    {
        [Test]
        public void It_adds_FakeRequest_and_reads_all_added_FakeRequests()
        {
            var requestedQueryRepository = new RequestedQueryRepository();

            var firstRequestedQuery = CreateRequestedQueryWith("A");
            var secondRequestedQuery = CreateRequestedQueryWith("B");

            requestedQueryRepository.Add(firstRequestedQuery);
            requestedQueryRepository.Add(secondRequestedQuery);

            var requestedQueries = requestedQueryRepository.GetAll();

            var expectedRequestedQueries = new List<FakeRequest>
            {
                firstRequestedQuery, secondRequestedQuery
            };

            Assert.IsTrue(requestedQueries.SequenceEqual(expectedRequestedQueries));
        }

        private static FakeRequest CreateRequestedQueryWith(string key)
        {
            var requestedQuery = new FakeRequest
            {
                Path = "/" + key + "/products",
                UrlParameters = new Parameters(new List<Parameter>
                {
                    new Parameter("param-" + key + "-1", "value-" + key + "-1"),
                    new Parameter("param-" + key + "-2", "value-" + key + "-2")
                })
            };

            return requestedQuery;
        }
    }
}
