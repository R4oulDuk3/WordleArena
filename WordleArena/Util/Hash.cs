using System.Security.Cryptography;
using System.Text;

namespace WordleArena.Util;

public static class Hash
{
    public static Domain.Hash ComputeSha256Hash(string rawData)
    {
        // Create a SHA256   
        using var sha256Hash = SHA256.Create();
        // ComputeHash - returns byte array  
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

        // Convert byte array to a string   
        var builder = new StringBuilder();
        foreach (var t in bytes)
        {
            builder.Append(t.ToString("x2"));
        }

        return new Domain.Hash(builder.ToString());
    }
}