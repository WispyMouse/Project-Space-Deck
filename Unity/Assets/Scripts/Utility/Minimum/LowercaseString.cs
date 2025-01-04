namespace SpaceDeck.Utility.Minimum
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A string that is promising that it is in all lowercase.
    /// 
    /// All ids and actions are done in lowercase. This promises
    /// that designers don't need to fuss about capitalization.
    /// </summary>
    public struct LowercaseString : IEquatable<string>, IEquatable<LowercaseString>
    {
        public readonly string Value;

        public LowercaseString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                this.Value = string.Empty;
            }
            else
            {
                this.Value = value.ToLower();
            }
        }

        public override string ToString()
        {
            return this.Value;
        }

        public static implicit operator LowercaseString(string text)
        {
            return new LowercaseString(text);
        }

        public static implicit operator string(LowercaseString lowercaseString)
        {
            return lowercaseString.Value;
        }

        public static List<LowercaseString> FromArray(string[] text)
        {
            List<LowercaseString> strings = new List<LowercaseString>();

            foreach (string textItem in text)
            {
                strings.Add(new LowercaseString(textItem));
            }

            return strings;
        }

        public bool Equals(string other)
        {
            return this.Value.Equals(other.ToLower());
        }

        public bool Equals(LowercaseString other)
        {
            return this.Value.Equals(other.Value);
        }

        public static bool operator ==(LowercaseString a, LowercaseString b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }
            else if (ReferenceEquals(b, null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(LowercaseString a, LowercaseString b)
        {
            return !(a == b);
        }

        public static LowercaseString operator +(LowercaseString a, LowercaseString b)
        {
            return a.Value + b.Value;
        }

        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }

            if (o is LowercaseString lowercaseString)
            {
                return this == lowercaseString;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
    }
}