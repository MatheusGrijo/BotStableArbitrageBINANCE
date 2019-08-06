
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


public class Utils
{
    public static string ByteToString(byte[] buff)
    {
        string sbinary = "";
        for (int i = 0; i < buff.Length; i++)
        {
            sbinary += buff[i].ToString("X2"); /* hex format */
        }
        return (sbinary);
    }

    public static string GenerateTimeStamp(DateTime value)
    {
        var dtOffset = new DateTimeOffset(value);
        return dtOffset.ToUnixTimeMilliseconds().ToString();
    }

    private static char[] HEX_DIGITS = new char[]{'0', '1', '2', '3', '4', '5',
            '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
    /// <summary>
    /// 生成32位大写MD5值
    /// </summary>
    public static String getMD5String(String str)
    {

        if (str == null || str.Trim().Length == 0)
        {
            return "";
        }
        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        MD5CryptoServiceProvider md = new MD5CryptoServiceProvider();
        bytes = md.ComputeHash(bytes);
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append(HEX_DIGITS[(bytes[i] & 0xf0) >> 4] + ""
                    + HEX_DIGITS[bytes[i] & 0xf]);
        }
        return sb.ToString();
    }



    public static long GetNonce()
    {
        return DateTime.Now.Ticks * 10 / TimeSpan.TicksPerMillisecond; // use millisecond timestamp or whatever you want
    }

    public static decimal percent(decimal value, decimal percent)
    {
        percent = percent / 100;
        return value + (percent * value);
    }

    public static string CalculateSignature(string text, string secretKey)
    {
        using (var hmacsha512 = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey)))
        {
            hmacsha512.ComputeHash(Encoding.UTF8.GetBytes(text));
            return string.Concat(hmacsha512.Hash.Select(b => b.ToString("x2")).ToArray()); // minimalistic hex-encoding and lower case
        }
    }

    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }
}
