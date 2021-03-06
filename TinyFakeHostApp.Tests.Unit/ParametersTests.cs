﻿using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using TinyFakeHostApp.RequestResponse;

namespace TinyFakeHostApp.Tests.Unit
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
        public void When_Parameters_and_key_value_pair_list_for_requested_parameters_are_equal_Matches_method_returns_true()
        {
            var requestedParameters = new List<KeyValuePair<string, StringValues>>
            {
                new KeyValuePair<string, StringValues> ("KeyB", "ValueB"), new KeyValuePair<string, StringValues>("KeyA", "ValueA")
            };

            Assert.IsTrue(_parameters.Matches(requestedParameters));
        }

        [Test]
        public void When_Parameters_and_key_value_pair_list_for_requested_parameters_are_not_equal_Matches_method_returns_false()
        {
            var requestedParameters = new List<KeyValuePair<string, StringValues>>
            {
                new KeyValuePair<string, StringValues> ("KeyB", "ValueA"), new KeyValuePair<string, StringValues>("KeyA", "ValueB")
            };

            Assert.IsFalse(_parameters.Matches(requestedParameters));
        }

        [Test]
        public void When_Parameters_value_is_ANYTHING_keyword_Matches_method_returns_true()
        {
            var parameters = new List<Parameter> { new Parameter("KeyA", "ValueA"), new Parameter("KeyB", "<<ANYTHING>>") };
            _parameters = new Parameters(parameters);
            var requestedParameters = new List<KeyValuePair<string, StringValues>>
            {
                new KeyValuePair<string, StringValues> ("KeyB", "ABCxyz"), new KeyValuePair<string, StringValues>("KeyA", "ValueA")
            };

            Assert.IsTrue(_parameters.Matches(requestedParameters));
        }

        [Test]
        public void When_Parameters_has_value_and_key_value_pair_list_for_requested_parameters_is_null_Matches_method_returns_false()
        {
            IEnumerable<KeyValuePair<string, StringValues>> requestedParameters = null;

            Assert.IsFalse(_parameters.Matches(requestedParameters));
        }

        [Test]
        public void When_Parameters_and_key_value_string_parameters_are_equal_Matches_method_returns_true()
        {
            const string keyValueStringParameters = "KeyB=ValueB&KeyA=ValueA";

            Assert.IsTrue(_parameters.Matches(keyValueStringParameters));
        }

        [Test]
        public void When_Parameters_and_key_value_string_parameters_are_not_equal_Matches_method_returns_false()
        {
            const string keyValueStringParameters = "KeyB=ValueA&&KeyA";

            Assert.IsFalse(_parameters.Matches(keyValueStringParameters));
        }

        [Test]
        public void When_Parameters_has_ANYTHING_keyword_partially_in_its_value_Matches_method_returns_true()
        {
            var parameters = new List<Parameter> { new Parameter("KeyA", "ValueA"), new Parameter("KeyB", "Val<<ANYTHING>>B") };
            _parameters = new Parameters(parameters);
            const string keyValueStringParameters = "KeyB=ValABCxyzB&KeyA=ValueA";

            Assert.IsTrue(_parameters.Matches(keyValueStringParameters));
        }

        [Test]
        public void When_Parameters_has_value_and_key_value_string_parameters_is_null_Matches_method_returns_false()
        {
            const string keyValueStringParameters = null;

            Assert.IsFalse(_parameters.Matches(keyValueStringParameters));
        }
    }
}
