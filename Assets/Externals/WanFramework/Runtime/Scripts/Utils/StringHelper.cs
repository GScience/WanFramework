using System;
using System.Buffers;
using TMPro;

namespace WanFramework.Utils
{
    /// <summary>
    /// 主要负责实现从数字到字符串的0GC转换
    /// </summary>
    public static class StringHelper
    {
        public static DisposableString ToStringNoGC(this int num)
        {
            var str = new DisposableString(16);
            ConvertFrom(ref str, num);
            return str;
        }
        
        public static void ConvertFrom(ref DisposableString str, int num)
        {
            var array = str.GetArray();
            var i = 0;
            var negative = false;
            if (num < 0)
            {
                negative = true;
                num = -num;
            }
            if (num == 0)
            {
                array[i++] = '0';
            }
            else
            {
                while (num > 0)
                {
                    array[i++] = (char)('0' + num % 10);
                    num /= 10;
                }
                Array.Reverse(array, 0, i);
                if (negative) array[i++] = '-';
            }
            str.Length = i;
        }
    }

    public static class TMPTextExtensions
    {
        public static void SetText(this TMP_Text text, DisposableString str)
        {
            text.SetText(str.GetArray(), 0, str.Length);
        }
    }
    public struct DisposableString : IDisposable
    {
        private static ArrayPool<char> _pool = ArrayPool<char>.Create();
        private readonly char[] _array;
        
        public int Length;
        
        public DisposableString(int capacity)
        {
            _array = _pool.Rent(capacity);
            Length = 0;
        }
        public char[] GetArray() => _array;
        public void Dispose()
        {
            if (_array != null)
                _pool.Return(_array);
        }
    }
}