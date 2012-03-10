using System;
using System.Collections.Generic;
using System.Text;

namespace Expressions
{
    public static class ObjectUtil
    {
        public static int CombineHashCodes(int hash1, int hash2)
        {
            return (((hash1 << 5) + hash1) ^ hash2);
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3)
        {
            return CombineHashCodes(CombineHashCodes(hash1, hash2), hash3);
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4)
        {
            return CombineHashCodes(CombineHashCodes(hash1, hash2), CombineHashCodes(hash3, hash4));
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5)
        {
            return CombineHashCodes(CombineHashCodes(hash1, hash2, hash3, hash4), hash5);
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6)
        {
            return CombineHashCodes(CombineHashCodes(hash1, hash2, hash3, hash4), CombineHashCodes(hash5, hash6));
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7)
        {
            return CombineHashCodes(CombineHashCodes(hash1, hash2, hash3, hash4), CombineHashCodes(hash5, hash6, hash7));
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8)
        {
            return CombineHashCodes(CombineHashCodes(hash1, hash2, hash3, hash4), CombineHashCodes(hash5, hash6, hash7, hash8));
        }

        public static int CombineHashCodes(int hash1, int hash2, int hash3, int hash4, int hash5, int hash6, int hash7, int hash8, int hash9)
        {
            return CombineHashCodes(CombineHashCodes(hash1, hash2, hash3, hash4), CombineHashCodes(CombineHashCodes(hash5, hash6, hash7, hash8), hash9));
        }
    }
}
