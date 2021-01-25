#region Utf8Json License https://github.com/neuecc/Utf8Json/blob/master/LICENSE
// MIT License
// 
// Copyright (c) 2017 Yoshifumi Kawai
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

namespace Elasticsearch.Net.Utf8Json.Internal.DoubleConversion
{
    // https://github.com/google/double-conversion/blob/master/double-conversion/diy-fp.cc
    // https://github.com/google/double-conversion/blob/master/double-conversion/diy-fp.h

    internal struct DiyFp
    {
        public const int kSignificandSize = 64;
        public const ulong kUint64MSB = 0x8000000000000000; // 0x80000000_00000000;

        // uint64_t f_;
        // int e_;
        // long f() const { return f_; }
        // int e() const { return e_; }
        // void set_f(long new_value) { f_ = new_value; }
        // void set_e(int new_value) { e_ = new_value; }

        // public field, not safe...
        public ulong f;
        public int e;

        public DiyFp(ulong significand, int exponent)
        {
            this.f = significand;
            this.e = exponent;
        }

        // this = this - other.
        // The exponents of both numbers must be the same and the significand of this
        // must be bigger than the significand of other.
        // The result will not be normalized.
        public void Subtract(ref DiyFp other)
        {
            f -= other.f;
        }

        // Returns a - b.
        // The exponents of both numbers must be the same and this must be bigger
        // than other. The result will not be normalized.
        public static DiyFp Minus(ref DiyFp a, ref DiyFp b)
        {
            DiyFp result = a;
            result.Subtract(ref b);
            return result;
        }

        public static DiyFp operator -(DiyFp lhs, DiyFp rhs)
        {
            return Minus(ref lhs, ref rhs);
        }

        // this = this * other.
        public void Multiply(ref DiyFp other)
        {
            // Simply "emulates" a 128 bit multiplication.
            // However: the resulting number only contains 64 bits. The least
            // significant 64 bits are only used for rounding the most significant 64
            // bits.
            const long kM32 = 0xFFFFFFFFU;
            ulong a = f >> 32;
            ulong b = f & kM32;
            ulong c = other.f >> 32;
            ulong d = other.f & kM32;
            ulong ac = a * c;
            ulong bc = b * c;
            ulong ad = a * d;
            ulong bd = b * d;
            ulong tmp = (bd >> 32) + (ad & kM32) + (bc & kM32);
            // By adding 1U << 31 to tmp we round the final result.
            // Halfway cases will be round up.
            tmp += 1U << 31;
            ulong result_f = ac + (ad >> 32) + (bc >> 32) + (tmp >> 32);
            e += other.e + 64;
            f = result_f;
        }

        // returns a * b;
        public static DiyFp Times(ref DiyFp a, ref DiyFp b)
        {
            DiyFp result = a;
            result.Multiply(ref b);
            return result;
        }

        public static DiyFp operator *(DiyFp lhs, DiyFp rhs)
        {
            return Times(ref lhs, ref rhs);
        }

        public void Normalize()
        {
            ulong significand = f;
            int exponent = e;

            // This method is mainly called for normalizing boundaries. In general
            // boundaries need to be shifted by 10 bits. We thus optimize for this case.
            const ulong k10MSBits = 0xFFC0000000000000; // UINT64_2PART_C(0xFFC00000, 00000000);
            while ((significand & k10MSBits) == 0)
            {
                significand <<= 10;
                exponent -= 10;
            }
            while ((significand & kUint64MSB) == 0)
            {
                significand <<= 1;
                exponent--;
            }
            f = significand;
            e = exponent;
        }

        public static DiyFp Normalize(ref DiyFp a)
        {
            DiyFp result = a;
            result.Normalize();
            return result;
        }
    }
}
