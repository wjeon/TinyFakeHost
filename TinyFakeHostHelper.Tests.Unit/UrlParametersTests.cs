using System.Collections.Generic;
using NUnit.Framework;
using Nancy;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Tests.Unit
{
    [TestFixture]
    public class UrlParametersTests
    {
        [Test]
        public void When_UrlParameters_and_dynamic_dictionary_for_requested_parameters_are_equal_Equals_method_returns_true()
        {
            var parameters = new List<UrlParameter> { new UrlParameter("KeyA", "ValueA"), new UrlParameter("KeyB", "ValueB") };

            var urlParameters = new UrlParameters(parameters);

            var requestedParameters = new DynamicDictionary { {"KeyB", "ValueB"}, {"KeyA", "ValueA"} };

            Assert.IsTrue(urlParameters.Equals(requestedParameters));
        }
    }
}
