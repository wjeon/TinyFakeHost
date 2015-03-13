using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using TinyFakeHostHelper.Asserters;
using TinyFakeHostHelper.Persistence;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Tests.Unit
{
    [TestFixture]
    public class FluentAsserterTests
    {
        private IRequestedQueryRepository _requestedQueryRepository;
        private FluentAsserter _fluentAsserter;
        private const string ResourcePath = "/resourcePath";
        private const string ParamKey = "param";
        private const string ParamValue = "value";

        [SetUp]
        public void SetUp()
        {
            SetRepositoryToReturnRequestedQueries();

            _fluentAsserter = new FluentAsserter(_requestedQueryRepository);
        }

        [Test]
        public void When_FluentAsserter_asserts_requested_query_incorrectly_with_wrong_resource_path_it_throws_assertion_exception()
        {
            Assert.Throws<Exceptions.AssertionException>(() =>
                _fluentAsserter
                    .Resource("/wrongResourcePath")
                    .WasRequested()
            );
        }

        [Test]
        public void When_FluentAsserter_asserts_requested_query_correctly_with_resource_path_and_parameter_it_does_not_throw_exception()
        {
            Assert.DoesNotThrow(() =>
                _fluentAsserter
                    .Resource(ResourcePath)
                    .WithParameters(ParamKey + "=" + ParamValue)
                    .WasRequested()
            );
        }

        private void SetRepositoryToReturnRequestedQueries()
        {
            _requestedQueryRepository = MockRepository.GenerateStub<IRequestedQueryRepository>();

            var requestedQueryWithResourcePathAndParameter = new FakeRequest
            {
                Path = ResourcePath,
                Parameters = new UrlParameters(new List<UrlParameter> { new UrlParameter(ParamKey, ParamValue) })
            };

            var requestedQueries = new List<FakeRequest>
            {
                requestedQueryWithResourcePathAndParameter
            };

            _requestedQueryRepository.Stub(s => s.GetAll()).Return(requestedQueries);
        }
    }
}
