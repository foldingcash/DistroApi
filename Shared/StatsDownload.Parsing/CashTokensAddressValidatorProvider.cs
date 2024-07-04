namespace StatsDownload.Parsing
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Security.Cryptography;
    using System.Text;
    using Core.Interfaces;
    using Microsoft.Extensions.Logging;

    public class CashTokensAddressValidatorProvider : ICashTokensAddressValidatorService
    {
        private readonly ILogger logger;

        public CashTokensAddressValidatorProvider(ILogger<CashTokensAddressValidatorProvider> logger)
        {
            this.logger = logger;
        }

        public string GetBitcoinAddress(string address)
        {
            return CashAddressToOldAddress(address, out bool _, out bool _);
        }

        public bool IsValidCashTokensAddress(string address)
        {
            try
            {
                CashAddressToOldAddress(address, out bool _, out bool _);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string CashAddressToOldAddress(string cashAddress, out bool isP2PKH, out bool main)
        {
            cashAddress = cashAddress.ToLower();
            if (cashAddress.Length != 54 && cashAddress.Length != 42 && cashAddress.Length != 50
                && cashAddress.Length != 55 && cashAddress.Length != 43 && cashAddress.Length != 51)
            {
                if (cashAddress.StartsWith("bchreg:"))
                {
                    throw new Exception("Decoding RegTest addresses is not implemented.");
                }

                throw new Exception("Address to be decoded is longer or shorter than expected.");
            }

            int afterPrefix;
            if (cashAddress.StartsWith("bitcoincash:"))
            {
                main = true;
                afterPrefix = 12;
            }
            else if (cashAddress.StartsWith("bchtest:"))
            {
                throw new Exception("Decoding testnet addresses is not supported.");
            }
            else if (cashAddress.StartsWith("bchreg:"))
            {
                throw new Exception("Decoding RegTest addresses is not implemented.");
            }
            else
            {
                if (cashAddress.IndexOf(":", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    main = true;
                    afterPrefix = 0;
                }
                else
                {
                    throw new Exception("Unexpected colon character.");
                }
            }

            int max = afterPrefix + 42;
            if (max != cashAddress.Length)
            {
                throw new Exception("Address to be decoded is longer or shorter than expected.");
            }

            var decodedBytes = new byte[42];
            for (int i = afterPrefix; i < max; i++)
            {
                int value = CashAddress[cashAddress[i]];
                if (value != -1)
                {
                    decodedBytes[i - afterPrefix] = (byte)value;
                }
                else
                {
                    throw new Exception("Address contains unexpected character.");
                }
            }

            if (PolyMod(decodedBytes, (ulong)(main ? 1058337025301 : 584719417569)) != 0)
            {
                throw new Exception("Address checksum doesn't match. Have you made a mistake while typing it?");
            }

            decodedBytes = ConvertBitsFiveToEight(decodedBytes);
            switch (decodedBytes[0])
            {
                case 0x10:
                    isP2PKH = true;
                    break;
                case 0x18:
                    isP2PKH = false;
                    break;
                default:
                    throw new Exception("Unexpected address byte.");
            }

            if (main && isP2PKH)
            {
                decodedBytes[0] = 0x00;
            }
            else if (main && !isP2PKH)
            {
                decodedBytes[0] = 0x05;
            }
            else if (!main && isP2PKH)
            {
                decodedBytes[0] = 0x6f;
            }
            else
                // Warning! Bigger than 0x80.
            {
                decodedBytes[0] = 0xc4;
            }

            using var hasher = SHA256.Create();
            byte[] checksum = hasher.ComputeHash(hasher.ComputeHash(decodedBytes, 0, 21));
            decodedBytes[21] = checksum[0];
            decodedBytes[22] = checksum[1];
            decodedBytes[23] = checksum[2];
            decodedBytes[24] = checksum[3];
            var ret = new StringBuilder(40);
            for (var numZeros = 0; numZeros < 25 && decodedBytes[numZeros] == 0; numZeros++)
            {
                ret.Append("1");
            }

            {
                var temp = new List<byte>(decodedBytes);
                // for 0xc4
                temp.Insert(0, 0);
                temp.Reverse();
                decodedBytes = temp.ToArray();
            }

            var retArr = new byte[40];
            var retIdx = 0;
            BigInteger baseChanger = BigInteger.Abs(new BigInteger(decodedBytes));
            var baseFiftyEight = new BigInteger(58);
            while (!baseChanger.IsZero)
            {
                baseChanger = BigInteger.DivRem(baseChanger, baseFiftyEight, out BigInteger modulo);
                retArr[retIdx++] = (byte)modulo;
            }

            for (retIdx--; retIdx >= 0; retIdx--)
            {
                ret.Append(Base58Charset[retArr[retIdx]]);
            }

            return ret.ToString();
        }

        private byte[] ConvertBitsFiveToEight(byte[] bytes)
        {
            var converted = new byte[1 + 20 + 4];
            int a1 = 0, a2 = 0;
            for (; a2 < 32; a1 += 5, a2 += 8)
            {
                converted[a1] = (byte)((bytes[a2] << 3) | (bytes[a2 + 1] >> 2));
                converted[a1 + 1] = (byte)(((bytes[a2 + 1] % 4) << 6) | (bytes[a2 + 2] << 1) | (bytes[a2 + 3] >> 4));
                converted[a1 + 2] = (byte)(((bytes[a2 + 3] % 16) << 4) | (bytes[a2 + 4] >> 1));
                converted[a1 + 3] = (byte)(((bytes[a2 + 4] % 2) << 7) | (bytes[a2 + 5] << 2) | (bytes[a2 + 6] >> 3));
                converted[a1 + 4] = (byte)(((bytes[a2 + 6] % 8) << 5) | bytes[a2 + 7]);
            }

            converted[a1] = (byte)((bytes[a2] << 3) | (bytes[a2 + 1] >> 2));
            if (bytes[a2 + 1] % 4 != 0)
            {
                throw new Exception("Invalid SLP Address.");
            }

            return converted;
        }

        private ulong PolyMod(byte[] input, ulong startValue = 1)
        {
            for (uint i = 0; i < 42; i++)
            {
                ulong c0 = startValue >> 35;
                startValue = ((startValue & 0x07ffffffff) << 5) ^ input[i];
                if ((c0 & 0x01) != 0)
                {
                    startValue ^= 0x98f2bc8e61;
                }

                if ((c0 & 0x02) != 0)
                {
                    startValue ^= 0x79b76d99e2;
                }

                if ((c0 & 0x04) != 0)
                {
                    startValue ^= 0xf33e5fb3c4;
                }

                if ((c0 & 0x08) != 0)
                {
                    startValue ^= 0xae2eabe2a8;
                }

                if ((c0 & 0x10) != 0)
                {
                    startValue ^= 0x1e4f43e470;
                }
            }

            return startValue ^ 1;
        }

        #region Crypto Constants

        private const string Base58Charset = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        private readonly sbyte[] CashAddress =
        {
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            15,
            -1,
            10,
            17,
            21,
            20,
            26,
            30,
            7,
            5,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            -1,
            29,
            -1,
            24,
            13,
            25,
            9,
            8,
            23,
            -1,
            18,
            22,
            31,
            27,
            19,
            -1,
            1,
            0,
            3,
            16,
            11,
            28,
            12,
            14,
            6,
            4,
            2,
            -1,
            -1,
            -1,
            -1,
            -1
        };

        #endregion
    }
}