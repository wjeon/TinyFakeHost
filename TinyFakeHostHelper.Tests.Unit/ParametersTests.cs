using System.Collections.Generic;
using NUnit.Framework;
using Nancy;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Tests.Unit
{
    [TestFixture]
    public class ParametersTests
    {
        [Test]
        public void When_Parameters_and_dynamic_dictionary_for_requested_parameters_are_equal_Equals_method_returns_true()
        {
            var parameters = new List<Parameter> { new Parameter("KeyA", "ValueA"), new Parameter("KeyB", "ValueB") };

            var urlParameters = new Parameters(parameters);

            var requestedParameters = new DynamicDictionary { {"KeyB", "ValueB"}, {"KeyA", "ValueA"} };

            Assert.IsTrue(urlParameters.Equals(requestedParameters));
        }
    }
}
