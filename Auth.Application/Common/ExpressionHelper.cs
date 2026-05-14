using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Auth.Application.Common
{
    public static class ExpressionHelper
    {
        public static bool IsValidMobile(this string Phone)
        {
            try
            {
                const string pattern = @"^09[0-9]{9}$";
                Regex reg = new Regex(pattern);
                return reg.IsMatch(Phone);
            }
            catch
            {
                return false;
            }
        }

        public static string RandomInt(this string i, int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "Size must be a positive integer.");
            }

            const string firstChars = "123456789";
            const string otherChars = "0123456789";

            using var crypto = RandomNumberGenerator.Create();
            var result = new char[size];

            result[0] = firstChars[GetRandomInt(crypto, firstChars.Length)];
            for (int index = 1; index < size; index++)
            {
                result[index] = otherChars[GetRandomInt(crypto, otherChars.Length)];
            }

            return new string(result);
        }

        private static int GetRandomInt(RandomNumberGenerator rng, int maxValue)
        {
            var data = new byte[4];
            rng.GetBytes(data);
            return (int)(BitConverter.ToUInt32(data, 0) % maxValue);
        }
    }
}

    
    
