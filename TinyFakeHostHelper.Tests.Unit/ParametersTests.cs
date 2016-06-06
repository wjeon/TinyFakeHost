using System.Collections.Generic;
using NUnit.Framework;
using Nancy;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Tests.Unit
{
    [TestFixture]
    public class ParametersTests
    {
        private Parameters _parameters;

        [SetUp]
        public void SetUp()
        {
            var parameters = new List<Parameter> { new Parameter("KeyA", "ValueA"), new Parameter("KeyB", "ValueB") };

            _parameters = new Parameters(parameters);
        }

        [Test]
        public void When_Parameters_and_dynamic_dictionary_for_requested_parameters_are_equal_Equals_method_returns_true()
        {
            var requestedParameters = new DynamicDictionary { {"KeyB", "ValueB"}, {"KeyA", "ValueA"} };

            Assert.IsTrue(_parameters.Equals(requestedParameters));
        }

        [Test]
        public void When_Parameters_and_dynamic_dictionary_for_requested_parameters_are_not_equal_Equals_method_returns_false()
        {
            var requestedParameters = new DynamicDictionary { { "KeyB", "ValueA" }, { "KeyA", "ValueB" } };

            Assert.IsFalse(_parameters.Equals(requestedParameters));
        }

        [Test]
        public void When_Parameters_and_key_value_string_parameters_are_equal_Equals_method_returns_true()
        {
            const string keyValueStringParameters = "KeyB=ValueB&KeyA=ValueA";

            Assert.IsTrue(_parameters.Equals(keyValueStringParameters));
        }

        [Test]
        public void When_Parameters_and_key_value_string_parameters_are_not_equal_Equals_method_returns_false()
        {
            const string keyValueStringParameters = "KeyB=ValueA&&KeyA";

            Assert.IsFalse(_parameters.Equals(keyValueStringParameters));
        }
    }
}
