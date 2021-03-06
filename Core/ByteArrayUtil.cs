﻿namespace Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    public class ByteArrayUtil
    {
        public static IEnumerable<byte[]> GetCombinations(int arrayLength, int numberOfBitsSet)
        {
            var bitlength = arrayLength * 8;
            foreach (var combo in CombinationsUtil.GetCombinations(bitlength, numberOfBitsSet))
            {
                yield return GetByteArrayWithValuesSet(arrayLength, combo);
            }            
        }

        public static byte[] GetByteArrayWithValuesSet(int length, params int[] bitsSet)
        {
            BitArray array = new BitArray(length * 8);

            foreach (var currentBit in bitsSet)
            {
                array.Set(currentBit, true);
            }

            return ByteArrayUtil.ConvertToByteArray(array);
        }

        private static byte[] ConvertToByteArray(BitArray bits)
        {
            if (bits.Count % 8 != 0)
            {
                throw new ArgumentException("illegal number of bits");
            }

            var result = new byte[bits.Count / 8];
            for (int i = 0; i < bits.Length / 8; i++)
            {
                byte b = 0;
                if (bits.Get(i * 8 + 0))
                {
                    b++;
                }
                if (bits.Get(i * 8 + 1))
                {
                    b += 2;
                }
                if (bits.Get(i * 8 + 2))
                {
                    b += 4;
                }
                if (bits.Get(i * 8 + 3))
                {
                    b += 8;
                }
                if (bits.Get(i * 8 + 4))
                {
                    b += 16;
                }
                if (bits.Get(i * 8 + 5))
                {
                    b += 32;
                }
                if (bits.Get(i * 8 + 6))
                {
                    b += 64;
                }
                if (bits.Get(i * 8 + 7))
                {
                    b += 128;
                }
                result[i] = b;
            }
            return result;
        }

        public static byte[] HexStringToByteArray(string source)
        {
            //remove blanks
            source = source.Trim();
            //check if source starts with common hex prefix and remove if needed
            source = source.StartsWith("0x") ? source.Remove(0, 2) : source;

            var bytes = new List<byte>();

            var startIndex = 0;
            if (source.Length % 2 == 1)
            {
                startIndex = 1;
                bytes.Add(ConvertHexDigitToByte(source[0]));
            }

            for (var index = startIndex; index < source.Length; index += 2)
            {
                var characterValue1 = ConvertHexDigitToByte(source[index]);
                var characterValue2 = ConvertHexDigitToByte(source[index + 1]);

                characterValue1 <<= 4;
                var currentValue = characterValue1;
                currentValue += characterValue2;
                bytes.Add(currentValue);
            }

            return bytes.ToArray();
        }

        private static byte ConvertHexDigitToByte(char hexDigit)
        {
            switch (hexDigit)
            {
                case '0':
                {
                    return 0;
                }
                case '1':
                {
                    return 1;
                }
                case '2':
                {
                    return 2;
                }
                case '3':
                {
                    return 3;
                }
                case '4':
                {
                    return 4;
                }
                case '5':
                {
                    return 5;
                }
                case '6':
                {
                    return 6;
                }
                case '7':
                {
                    return 7;
                }
                case '8':
                {
                    return 8;
                }
                case '9':
                {
                    return 9;
                }
                case 'a':
                case 'A':
                {
                    return 10;
                }
                case 'b':
                case 'B':
                {
                    return 11;
                }
                case 'c':
                case 'C':
                {
                    return 12;
                }
                case 'd':
                case 'D':
                {
                    return 13;
                }
                case 'e':
                case 'E':
                {
                    return 14;
                }
                case 'f':
                case 'F':
                {
                    return 15;
                }
                default:
                {
                    throw new ArgumentException("Passed in a character that wasn't 0-9 or a-f");
                }
            }
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            return ByteArrayToHexString(ba, 0, ba.Length);
        }

        public static string ByteArrayToHexString(byte[] ba, int offset, int count)
        {
            var result = new StringBuilder();
            var usedcount = 0;
            for (var i = offset; usedcount < count; i++, usedcount++)
            {
                result.Append(string.Format("{0:X2}", ba[i]));
            }
            return "0x"+result.ToString();
        }
    }
}
