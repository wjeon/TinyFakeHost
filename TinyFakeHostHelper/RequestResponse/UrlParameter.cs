namespace TinyFakeHostHelper.RequestResponse
{
    public class UrlParameter
    {
        public UrlParameter(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }
        public string Value { get; private set; }

        protected bool Equals(UrlParameter other)
        {
            return string.Equals(Key, other.Key) && string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UrlParameter)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public static bool operator ==(UrlParameter left, UrlParameter right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UrlParameter left, UrlParameter right)
        {
            return !Equals(left, right);
        }
    }
}