using System;

namespace DES
{
    public class Des
    {
        private const int BLOCK_SIZE = 64;
        private const int QUANTITY_OF_ROUNDS = 16;

        public string Code(string message, string key, CipherMode mode)
        {
            var key1 = Permute(BinaryConverter.StringToBinary(key, BLOCK_SIZE), Matrix.PC1);      //Преобразуем ключ в двоичный вид и трансформируем с помощью матрицы РС1
            var binaryKeys = GetBinaryKeys(key1);                                                    // Получаем 16 ключей путём сдвига

            // Преобразуем сообщение в двоичный вид и, при необходимости, дополняем до 64 бит
            var workString = BinaryConverter.StringToBinary(message, BLOCK_SIZE);

            // Разбиваем двоичную строку на блоки по 64 бита
            var blocksCount = workString.Length / BLOCK_SIZE;
            var blocks = new string[blocksCount];
            //Трансформируем каждый блок с помощью матрицы IP
            for (int i = 0; i < blocks.Length; i++)
            {
                if (workString.Length > 64)
                {
                    blocks[i] = Permute(workString.Substring(i * BLOCK_SIZE, BLOCK_SIZE), Matrix.IP);
                }
                else
                {
                    blocks[i] = Permute(workString, Matrix.IP);
                }
            }

            // Для каждого блока из 64-х бит выполняем 16 раундов преобразования
            for (int j = 0; j < blocksCount; j++)
            {
                for (int i = 0; i < QUANTITY_OF_ROUNDS; i++)
                {
                    blocks[j] = EncodeDES_One_Round(blocks[j], binaryKeys[(mode == CipherMode.Encrypt) ? i : 15 - i]);
                }
            }

            // Поскольку после 16-го раунда нам нужен блок вида RL, а мы имеем LR, то циклически сдвигаем блок на 32 бита влево
            // Затем трансформируем с помощью обратной матрицы IPo 
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i] = Rol(blocks[i], 32);
                blocks[i] = Permute(blocks[i], Matrix.IPo);
            }

            string binResult = null;
            foreach (var t in blocks)
            {
                binResult += t;
            }

            return BinaryConverter.BinaryToString(binResult, 8);
        }


        private string f(string R, string key)
        {
            string outStr = null;
            var ER = Permute(R, Matrix.E); // Дополняем блок R до 6 бит (аналог функции Е)

            // Находим Kn + E(Rn-1)
            string ER_XOR_Key = null;
            for (int i = 0; i < ER.Length; i++)
            {
                ER_XOR_Key += Convert.ToInt32(ER[i].ToString()) ^ Convert.ToInt32(key[i].ToString());
            }

            // Разбиваем ключ на 8 групп по 6 бит для дальнейшей работы с матрицой S
            var ER_XOR_Key_Group_Six_Bits = new string[8];
            for (int i = 0; i < 8; i++)
            {
                ER_XOR_Key_Group_Six_Bits[i] = ER_XOR_Key.Substring(i * 6, 6);
            }

            // Для каждой группы вычисляем новое значечие в матрице и преобразуем его в двоичный вид
            for (int i = 0; i < 8; i++)
            {
                var row = BinaryConverter.BinaryToDecimal($"{ER_XOR_Key_Group_Six_Bits[i][0]}{ER_XOR_Key_Group_Six_Bits[i][5]}"); //Находим номер строки i-го блока матрицы
                var col = BinaryConverter.BinaryToDecimal(ER_XOR_Key_Group_Six_Bits[i].Substring(1, 4));             //Находим номер столбца i-го блока матрицы
                outStr += BinaryConverter.DecimalToBinary(Matrix.S[i][16 * row + col]);                                                          // Поскольку данные в матрице представленны в виде массива строк, то
                                                                                                                                                 // необходимо умножить номер строки на 16 (поскольку всего 64 символа,
                                                                                                                                                 // а номер строки получаем от 0 до 3)
            }

            // Трансформируем с помощью матрицы P 
            outStr = Permute(outStr, Matrix.P);
            
            return outStr;
        }
        
        private string EncodeDES_One_Round(string input, string key)
        {
            string L = input.Substring(0, input.Length / 2);
            string R = input.Substring(input.Length / 2, input.Length / 2);

            return (R + XOR(L, f(R, key)));
        }

        private string XOR(string s1, string s2)
        {
            string result = "";
            for (int i = 0; i < s1.Length; i++)
            {
                result += Convert.ToInt32(s1[i].ToString()) ^ Convert.ToInt32(s2[i].ToString());
            }

            return result;
        }

        private string Permute(string block, int[] matrix)
        {
            string outStr = null;
            foreach (var t in matrix)
            {
                outStr += block[t - 1];
            }

            return outStr;
        }

        private string[] GetBinaryKeys(string key)
        {
            var keys = new string[16];
            var c0 = key.Substring(0, 28);
            var d0 = key.Substring(28, 28);

            for (int i = 0; i < 16; i++)
            {
                c0 = Rol(c0, Matrix.KeyShift[i]);
                d0 = Rol(d0, Matrix.KeyShift[i]);

                keys[i] = Permute(c0 + d0, Matrix.PC2);
            }

            return keys;
        }

        private static string Rol(string block, int bits)
        {
            return block.Substring(bits, block.Length - bits) + block.Substring(0, bits);
        }
    }
}