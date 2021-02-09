using System;
using System.Collections;
using System.Collections.Generic;

namespace hashes
{
    public class ReadonlyBytes : IEnumerable<byte>
    {
        
        byte[] collection;

        int count = 0;

        int hash = -1;

        public ReadonlyBytes(params byte[] values)
        {
            if (values == null)
                throw new ArgumentNullException();
            collection = new byte[values.Length];     
            for (int i = 0; i < values.Length; i++)
                collection[i] = values[i];
            count = collection.Length;
        }

        public IEnumerator<byte> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
                yield return (byte)collection[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Length
        {
            get { return collection.Length; }
        }

        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= count) throw new IndexOutOfRangeException();
                return collection[index];
            }
        }

        public override int GetHashCode()
        {
            const long prime0 = 17;
            const long prime1 = 2147483029; // prime1 < 2147483648 == 2^31
            const long prime2 = 331; // 256 < prime2 < 512

            if (hash == -1) // lazy calculation
            {
                long t = prime0;
                foreach (var element in collection)
                    t = (t * prime2 + element) % prime1;
                hash = (int)t; // 0 <= t < prime1 < 2^31
            }
            return hash;
        }

        public override string ToString()
        {
            if (count == 0)
                return "[]";
            var result = "[";
            for (var i = 0; i < count; i++)
            {
                result += collection[i].ToString();
                if (i + 1 < count)
                    result += ", ";
                else result += "]";
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            var readonlyBytes = obj as ReadonlyBytes;
            if (obj != null && obj.GetType() == typeof(ReadonlyBytes))
                if (this.ToString() == readonlyBytes.ToString())
                    return true;
            return false;
        }
    }
}