﻿namespace StatsDownload.Core
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;

    public class BitcoinAddressValidatorProvider : IBitcoinAddressValidatorService
    {
        private const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        private const int Size = 25;

        public bool IsValidBitcoinAddress(string bitcoinAddress)
        {
            if (bitcoinAddress.Length < 26 || bitcoinAddress.Length > 35)
            {
                return false;
            }
            byte[] decoded = DecodeBase58(bitcoinAddress);

            if (decoded == null)
            {
                return false;
            }

            byte[] d1 = Hash(SubArray(decoded, 0, 21));
            byte[] d2 = Hash(d1);
            if (!SubArray(decoded, 21, 4).SequenceEqual(SubArray(d2, 0, 4)))
            {
                return false;
            }
            return true;
        }

        private byte[] DecodeBase58(string input)
        {
            var output = new byte[Size];
            foreach (char t in input)
            {
                int p = Alphabet.IndexOf(t);
                if (p == -1)
                {
                    return null;
                }
                int j = Size;
                while (--j > 0)
                {
                    p += 58 * output[j];
                    output[j] = (byte)(p % 256);
                    p /= 256;
                }
                if (p != 0)
                {
                    return null;
                }
            }
            return output;
        }

        private byte[] Hash(byte[] bytes)
        {
            var hasher = new SHA256Managed();
            return hasher.ComputeHash(bytes);
        }

        private T[] SubArray<T>(T[] data, int index, int length)
        {
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}