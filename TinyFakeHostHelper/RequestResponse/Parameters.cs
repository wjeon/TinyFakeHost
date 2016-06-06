using System.Collections.Generic;
using System.Linq;
using Nancy;

namespace TinyFakeHostHelper.RequestResponse
{
    public class Parameters : List<Parameter>
    {
        public Parameters() : base(new List<Parameter>()) { }
        public Parameters(IEnumerable<Parameter> parameters) : base(parameters) { }

        public bool Equals(DynamicDictionary query)
        {
            if (query.Count == 0 && Count == 0) return true;

            var parameters = query.Keys.Select(key => new Parameter(key, query[key].ToString())).ToList();

            return this.OrderBy(r => r.Key).SequenceEqual(parameters.OrderBy(r => r.Key));
        }

        public bool Equals(string value)
        {
            var parameters = value.Split('&').Select(v => v.Split('=')).Select(p => new Parameter(p[0], p.Length > 1 ? p[1] : null)).ToList();
            return this.OrderBy(r => r.Key).SequenceEqual(parameters.OrderBy(r => r.Key));
        }

        public override string ToString()
        {
            return string.Join("&", this.Select(p => p.Key + "=" + p.Value));
        }
    }
}