﻿using System;

namespace TinyFakeHostHelper.RequestResponse
{
    [Obsolete("Please use \"Parameter\" class instead")]
    public class UrlParameter : Parameter
    {
        public UrlParameter(string key, string value) : base(key, value)
        {
        }
    }
}