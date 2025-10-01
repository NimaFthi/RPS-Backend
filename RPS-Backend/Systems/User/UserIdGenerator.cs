namespace RPS_Backend.Systems.User;

using System;
using System.Security.Cryptography;
using System.Text;

public static class UserIdGenerator
{
    private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public static string GenerateUserId(int length = 22)
    {
        var guidBytes = Guid.NewGuid().ToByteArray();

        Span<byte> secureBytes = stackalloc byte[16];
        RandomNumberGenerator.Fill(secureBytes);
        for (int i = 0; i < 16; i++)
            guidBytes[i] ^= secureBytes[i];

        return ToBase62(guidBytes, length);
    }

    private static string ToBase62(ReadOnlySpan<byte> bytes, int length)
    {
        var sb = new StringBuilder();
        var value = new System.Numerics.BigInteger(bytes, isUnsigned: true, isBigEndian: false);

        while (value > 0 && sb.Length < length)
        {
            value = System.Numerics.BigInteger.DivRem(value, 62, out var remainder);
            sb.Insert(0, Base62Chars[(int)remainder]);
        }

        // Pad if shorter
        while (sb.Length < length)
            sb.Insert(0, '0');

        return sb.ToString();
    }
}
