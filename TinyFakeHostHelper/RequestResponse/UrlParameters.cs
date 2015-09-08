using System;
using System.Collections.Generic;

namespace TinyFakeHostHelper.RequestResponse
{
    [Obsolete("Please use \"Parameters\" class instead")]
    public class UrlParameters : Parameters
    {
        public UrlParameters() : base(new List<Parameter>()) { }
        public UrlParameters(IEnumerable<Parameter> parameters) : base(parameters) { }
    }
}