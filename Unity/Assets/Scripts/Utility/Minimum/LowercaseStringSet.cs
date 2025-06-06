namespace SpaceDeck.Utility.Minimum
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime;

    public struct LowercaseStringSet : IEquatable<LowercaseStringSet>
    {
        private readonly HashSet<LowercaseString> _Strings;
        public ICollection<LowercaseString> Strings => _Strings;

        /// <summary>
        /// If <see cref="Strings"/> contains exactly one item,
        /// this holds that value.
        /// </summary>
        public readonly LowercaseString? OnlyValue;

        public LowercaseStringSet(string tags)
        {
            this._Strings = new HashSet<LowercaseString>();

            string[] split = tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (string splitString in split)
            {
                this._Strings.Add(splitString.Trim());
            }

            if (split.Length == 1)
            {
                this.OnlyValue = split[0].Trim();
            }
            else
            {
                this.OnlyValue = null;
            }
        }

        public LowercaseStringSet(IEnumerable<string> tags)
        {
            _Strings = new HashSet<LowercaseString>();
            foreach (string curString in tags)
            {
                _Strings.Add(curString);
            }

            if (_Strings.Count == 1)
            {
                this.OnlyValue = _Strings.GetEnumerator().Current;
            }
            else
            {
                this.OnlyValue = null;
            }
        }

        public bool Equals(LowercaseStringSet other)
        {
            if (this.Strings.Count != other.Strings.Count)
            {
                return false;
            }

            foreach (LowercaseString curString in this.Strings)
            {
                if (!other._Strings.Contains(curString))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator ==(LowercaseStringSet a, LowercaseStringSet b)
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

        public static bool operator !=(LowercaseStringSet a, LowercaseStringSet b)
        {
            return !(a == b);
        }

        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }

            if (o is LowercaseStringSet lowercaseStringSet)
            {
                return this == lowercaseStringSet;
            }

            return false;
        }

        public override int GetHashCode()
        {
            HashCode hashCode = new HashCode();

            foreach (LowercaseString curString in this.Strings)
            {
                hashCode.Add(curString.GetHashCode());
            }

            return hashCode.ToHashCode();
        }

        public bool Contains(LowercaseStringSet other)
        {
            return this._Strings.Overlaps(other.Strings);
        }

        public override string ToString()
        {
            return String.Join(',', this._Strings);
        }
    }
}