using System;

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
    }
}
