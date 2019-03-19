using System;
using System.Collections.Generic;

namespace TinyFakeHostApp.RequestResponse
{
    [Obsolete("Please use \"Parameters\" class instead")]
    public class UrlParameters : Parameters
    {
        public UrlParameters() : base(new List<Parameter>()) { }
        public UrlParameters(IEnumerable<Parameter> parameters) : base(parameters) { }
    }
}