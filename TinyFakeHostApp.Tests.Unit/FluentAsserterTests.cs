using System.Collections.Generic;
using FakeItEasy;
using NUnit.Framework;
using TinyFakeHostApp.Asserters;
using TinyFakeHostApp.Persistence;
using TinyFakeHostApp.RequestResponse;
using AssertionException = TinyFakeHostApp.Exceptions.AssertionException;

namespace TinyFakeHostApp.Tests.Unit
{
    [TestFixture]
    public class FluentAsserterTests
    {
        private IRequestedQueryRepository _requestedQueryRepository;
        private FluentAsserter _fluentAsserter;
        private const string ResourcePathWihUrlParamOnly = "/resourcePathWihUrlParamOnly";
        private const string ResourcePathWihFormParamOnly = "/resourcePathWihFormParamOnly";
        private const string UrlParamKey = "urlparam";
        private const string UrlParamValue = "urlvalue";
        private const string FormParamKey = "formparam";
        private const string FormParamValue = "formvalue";
        private const string ResourcePathOnly = "/resourcePathOnly";

        [SetUp]
        public void SetUp()
        {
            SetRepositoryToReturnRequestedQueries();

            _fluentAsserter = new FluentAsserter(_requestedQueryRepository);
        }

        [Test]
        public void When_FluentAsserter_asserts_requested_query_incorrectly_with_wrong_resource_path_it_throws_assertion_exception()
        {
            Assert.Throws<AssertionException>(() =>
                _fluentAsserter
                    .Resource("/wrongResourcePath")
                    .WasRequested()
            );
        }

        [Test]
        public void When_FluentAsserter_asserts_requested_query_incorrectly_with_wrong_parameter_it_throws_assertion_exception()
        {
            Assert.Throws<AssertionException>(() =>
                _fluentAsserter
                    .Resource(ResourcePathWihUrlParamOnly)
                    .WithUrlParameters("param=wrong+parameter")
                    .WasRequested()
            );
        }

        [Test]
        public void When_FluentAsserter_asserts_requested_query_correctly_with_resource_path_and_url_parameter_it_does_not_throw_exception()
        {
            Assert.DoesNotThrow(() =>
                _fluentAsserter
                    .Resource(ResourcePathWihUrlParamOnly)
                    .WithUrlParameters(UrlParamKey + "=" + UrlParamValue)
                    .WasRequested()
            );
        }

        [Test]
        public void When_FluentAsserter_asserts_requested_query_correctly_with_resource_path_and_form_parameter_it_does_not_throw_exception()
        {
            Assert.DoesNotThrow(() =>
                _fluentAsserter
                    .Resource(ResourcePathWihFormParamOnly)
                    .WithFormParameters(FormParamKey + "=" + FormParamValue)
                    .WasRequested()
            );
        }

        [Test]
        public void When_FluentAsserter_asserts_requested_query_correctly_with_resource_path_only_it_does_not_throw_exception()
        {
            Assert.DoesNotThrow(() =>
                _fluentAsserter
                    .Resource(ResourcePathOnly)
                    .WasRequested()
            );
        }

        private void SetRepositoryToReturnRequestedQueries()
        {
            _requestedQueryRepository = A.Fake<IRequestedQueryRepository>();

            var requestedQueryWithResourcePathAndUrlParameter = new FakeRequest
            {
                Path = ResourcePathWihUrlParamOnly,
                UrlParameters = new Parameters(new List<Parameter> { new Parameter(UrlParamKey, UrlParamValue) })
            };

            var requestedQueryWithResourcePathAndFormParameter = new FakeRequest
            {
                Path = ResourcePathWihFormParamOnly,
                FormParameters = new Parameters(new List<Parameter> { new Parameter(FormParamKey, FormParamValue) })
            };

            var requestedQueryWithResourcePathOnly = new FakeRequest { Path = ResourcePathOnly };

            var requestedQueries = new List<FakeRequest>
            {
                requestedQueryWithResourcePathAndUrlParameter,
                requestedQueryWithResourcePathAndFormParameter,
                requestedQueryWithResourcePathOnly
            };

            A.CallTo(() => _requestedQueryRepository.GetAll()).Returns(requestedQueries);
        }
    }
}
