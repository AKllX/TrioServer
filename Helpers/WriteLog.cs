using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using TrioServer.Radios;
using TrioServer.Communication;

namespace TrioServer.Helpers
{
    public static class WriteLog
    {
        public static void AppendLog(RadioMessage m)
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "WriteLines.txt"), true))
            {
                outputFile.WriteLine(m.RadioSerialNumber + ": " + BitConverter.ToString(m.FullPacket));
            }
        }
    }
}
