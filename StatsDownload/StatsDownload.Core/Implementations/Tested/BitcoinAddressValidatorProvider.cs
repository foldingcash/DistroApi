namespace StatsDownload.Core.Implementations.Tested
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;

    using StatsDownload.Core.Interfaces;

    public class BitcoinAddressValidatorProvider : IBitcoinAddressValidatorService
    {
        public bool IsValidBitcoinAddress(string bitcoinAddress)
        {
            if (IsInvalidLength(bitcoinAddress))
            {
                return false;
            }

            byte[] decoded = DecodeBase58(bitcoinAddress);

            if (decoded == null)
            {
                return false;
            }

            if (IsInvalidCheckSum(decoded))
            {
                return false;
            }

            return true;
        }

        private byte[] DecodeBase58(string input)
        {
            var output = new byte[Constants.BitcoinAddress.Size];

            foreach (char t in input)
            {
                int p = Constants.BitcoinAddress.Alphabet.IndexOf(t);

                if (p == -1)
                {
                    return null;
                }

                int j = Constants.BitcoinAddress.Size;

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
            using (var hasher = new SHA256Managed())
            {
                return hasher.ComputeHash(bytes);
            }
        }

        private bool IsInvalidCheckSum(byte[] decoded)
        {
            byte[] d1 = Hash(SubArray(decoded, 0, 21));
            byte[] d2 = Hash(d1);
            return !SubArray(decoded, 21, 4).SequenceEqual(SubArray(d2, 0, 4));
        }

        private bool IsInvalidLength(string bitcoinAddress)
        {
            return bitcoinAddress.Length < 26 || bitcoinAddress.Length > 35;
        }

        private byte[] SubArray(byte[] data, int index, int length)
        {
            var result = new byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}