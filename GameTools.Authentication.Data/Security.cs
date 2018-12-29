using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GameTools.Authentication.Data
{
    public class Security
    {
        private int _iterations;
        private int _length;
        internal Security(int iterations, int length)
        {
            _iterations = iterations;
            _length = length;
        }

        internal byte[] GetHash(string inputString)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(inputString, GenerateSalt(), _iterations))
            {
                return deriveBytes.GetBytes(_length);
            }
        }

        private byte[] GenerateSalt()
        {
            var bytes = new byte[_length];
            using(var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return bytes;
        }
    }
}
