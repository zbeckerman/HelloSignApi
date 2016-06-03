﻿#if FULLFX
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelloSignApi
{
    static class Hasher
    {
        public static string GetHMACSHA256Hash(byte[] key, byte[] input)
        {
            using (var algo = new System.Security.Cryptography.HMACSHA256(key))
            {
                var hash = BitConverter.ToString(algo.ComputeHash(input));
                return hash.Replace("-", "");
            }

            //using System.Runtime.InteropServices.WindowsRuntime;
            //using Windows.Security.Cryptography.Core;
            //var macProv = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);
            //CryptographicKey hmacKey = macProv.CreateKey(key.AsBuffer());
            //var hmac = CryptographicEngine.Sign(hmacKey, input.AsBuffer());
            //var hash = BitConverter.ToString(hmac.ToArray());
            //return hash.Replace("-", "");
        }
    }
}

#endif