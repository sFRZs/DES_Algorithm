using System;
using System.Text;

namespace DES
{
    public static class BinaryConverter
    {
        public static int BinaryToDecimal(string binaryNumber)
        {
            Int32 decimalNumber = 0;

            for (int i = 0; i < binaryNumber.Length; i++)
            {
                if (binaryNumber[binaryNumber.Length - i - 1] == '0')
                {
                    continue;
                }

                decimalNumber += (int) Math.Pow(2, i);
            }

            return decimalNumber;
        }

        public static string DecimalToBinary(int number, int lengthOutNumber = 4)
        {
            Int32 temp = 0;
            String binaryNumber = "";

            while (number > 0)
            {
                temp = number % 2;
                number /= 2;
                binaryNumber = temp + binaryNumber;
            }

            if (binaryNumber.Length < lengthOutNumber)
            {
                var paddingKey = lengthOutNumber - (binaryNumber.Length % lengthOutNumber);
                for (int i = 0; i < paddingKey; i++)
                {
                    binaryNumber = binaryNumber.Insert(0, "0");
                }
            }

            return binaryNumber;
        }

        public static string StringToBinary(string input, int size)
        {
            string binStr = "";

            var tmp = Encoding.GetEncoding(1251).GetBytes(input);
            foreach (var b in tmp)
            {
                binStr += DecimalToBinary(b, 8);
            }

            if (binStr.Length < size)
            {
                var paddingKey = size - (binStr.Length % size);
                for (int i = 0; i < paddingKey; i++)
                {
                    binStr = binStr.Insert(0, "0");
                }
            }

            return binStr;
        }

        public static string BinaryToString(string input, int binLength)
        {
            var a = input.Length / 8;
            var byteString = new Byte[a];
            for (int i = 0; i < a; i++)
            {
                byteString[i] = (byte) BinaryToDecimal(input.Substring(i * binLength, binLength));
            }
            
            return Encoding.GetEncoding(1251).GetString(byteString);
        }
    }
}