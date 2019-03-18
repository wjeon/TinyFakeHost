using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using TinyFakeHostHelper.RequestResponse;

namespace TinyFakeHostHelper.Extensions
{
    public static class KeyValuePairExtensions
    {
        public static Parameters ToParameters(this IEnumerable<KeyValuePair<string, StringValues>> parameters)
        {
            return new Parameters(parameters.Select(p => new Parameter(p.Key, p.Value)));
        }

        public static IEnumerable<KeyValuePair<string, StringValues>> Form(this HttpRequest request)
        {
            IEnumerable<KeyValuePair<string, StringValues>> formCollection;

            try
            {
                formCollection = request.Form;
            }
            catch(InvalidOperationException)
            {
                formCollection = new List<KeyValuePair<string, StringValues>>();
            }

            return formCollection;
        }
    }
}
