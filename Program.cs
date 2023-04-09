using System;
using System.Text;
using DES;

class Program
{
    public static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var des = new Des();
        var en = des.Code("Привет", "мир", CipherMode.Encrypt);
        Console.WriteLine(en);
        var de = des.Code(en, "мир", CipherMode.Decrypt);
        Console.WriteLine(de);
    }
}