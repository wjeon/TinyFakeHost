using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;
using TinyFakeHostHelper.Extensions;

namespace TinyFakeHostHelper.RequestResponse
{
    public class Parameters : List<Parameter>
    {
        public Parameters() : base(new List<Parameter>()) { }
        public Parameters(IEnumerable<Parameter> parameters) : base(parameters) { }

        public bool Matches(IEnumerable<KeyValuePair<string, StringValues>> query)
        {
            var keyValuePairs = query ?? Enumerable.Empty<KeyValuePair<string, StringValues>>();

            if (!keyValuePairs.Any() && Count == 0) return true;

            if (!keyValuePairs.Any() || Count == 0) return false;

            var parameters = keyValuePairs.Select(q => new Parameter(q.Key, q.Value)).ToList();

            return SequenceMatch(parameters);
        }

        public bool Matches(string value)
        {
            if (string.IsNullOrEmpty(value) && Count == 0) return true;

            if (string.IsNullOrEmpty(value) || Count == 0) return false;

            var parameters = value.Split('&').Select(v => v.Split('=')).Select(p => new Parameter(p[0], p.Length > 1 ? p[1] : null)).ToList();

            return SequenceMatch(parameters);
        }

        private bool SequenceMatch(IEnumerable<Parameter> parameters)
        {
            if (Count != parameters.Count()) return false;

            var orderedThis = this.OrderBy(r => r.Key).ToList();
            var orderedParameters = parameters.OrderBy(r => r.Key).ToList();

            for (var i = 0; i < Count; i++)
            {
                if (orderedThis[i].Key != orderedParameters[i].Key || !orderedThis[i].Value.Matches(orderedParameters[i].Value))
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            return string.Join("&", this.Select(p => p.Key + "=" + p.Value));
        }
    }
}