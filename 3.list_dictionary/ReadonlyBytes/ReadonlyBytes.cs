using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace hashes
{
    public class ReadonlyBytes : IEnumerable<byte>
    {
        private readonly byte[] _data;
        private readonly int _hash;

        public ReadonlyBytes(params byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            _data = data;
            _hash = GetHashCodeInternal();
        }

        public int Length => _data.Length;

        public byte this[int index] => _data[index];

        public IEnumerator<byte> GetEnumerator()
        {
            foreach (var x in _data)
                yield return x;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", _data)}]";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj == null || GetType() != obj.GetType())
                return false;

            if (obj is ReadonlyBytes other)
            {
                if (Length != other.Length)
                    return false;

                for (var i = 0; i < Length; i++)
                {
                    if (this[i] != other[i])
                        return false;
                }

                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        private int GetHashCodeInternal()
        {
            if (_data.Length == 0)
                return 0;

            unchecked
            {
                const uint fnvOffset = 2166136261;
                const uint fnvPrime = 16777619;

                uint hash = fnvOffset;

                for (int i = 0; i < _data.Length; i++)
                {
                    hash ^= _data[i];
                    hash *= fnvPrime;
                }

                return (int)hash;
            }
        }
    }
}
