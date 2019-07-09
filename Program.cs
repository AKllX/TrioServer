using System;
using System.Text;

namespace TrioServer
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando...");
            Core.Initialize();
            Console.WriteLine("Servidor iniciado com sucesso");
            while(true)
            {

            }
        }

        public static double GetCurrentTimestamp()
        {
            TimeSpan ts = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0));
            return ts.TotalSeconds;
        }

        public static byte CheckSum8Xor(byte[] _PacketData, int PacketLength)
        {
            Byte _CheckSumByte = 0x00;
            for (int i = 0; i < PacketLength; i++)
                _CheckSumByte ^= _PacketData[i];
            return _CheckSumByte;
        }

        public static int GetSerialNumberFromBytes(byte [] b)
        {
            if (b.Length != 3)
            {
                throw new FormatException("Vetor de conversão fora do formato");
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                for(int i = 0; i < b.Length; i++)
                {
                    sb.Append(b[i].ToString("x2"));
                }
                return Convert.ToInt32(sb.ToString());
            }

        }
    }
}
