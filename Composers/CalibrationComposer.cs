using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

using TrioServer.Sessions;

namespace TrioServer.Composers
{
    public class CalibrationComposer
    {
        public static MemoryStream Serialize(Session mSession)
        {
            mSession.AuthMessageCounter++;
            MemoryStream packet = new MemoryStream();
            packet.WriteByte(0xc0);
            packet.WriteByte(0x00);
            packet.Write(mSession.Channel.SerialNumberParse());
            packet.WriteByte(0x00);
            packet.WriteByte(0x00);
            packet.WriteByte(0x01);
            packet.WriteByte(0x50); 
            packet.WriteByte(mSession.PacketTick()); //Packet Counter
            packet.WriteByte(0x00);
            packet.WriteByte(0x32);
            packet.WriteByte(0x00);
            packet.WriteByte(0x2f);
            packet.WriteByte(0x00);
            packet.WriteByte(0x30);
            packet.WriteByte(0x00);
            packet.WriteByte(0x31);
            packet.WriteByte(0x00);
            packet.WriteByte(0x33);
            packet.WriteByte(0x00);
            packet.WriteByte(0x34);
            packet.WriteByte(0x00);
            packet.WriteByte(0x35);
            packet.WriteByte(0x00);
            packet.WriteByte(0xad);
            packet.WriteByte(0x00);
            packet.WriteByte(0xae);
            packet.WriteByte(0x01);
            packet.WriteByte(0x07);
            packet.WriteByte(0x01);
            packet.WriteByte(0x08);
            packet.WriteByte(0x01);
            packet.WriteByte(0x09);
            packet.WriteByte(0x01);
            packet.WriteByte(0x0a);
            packet.WriteByte(0x01);
            packet.WriteByte(0x12);
            packet.WriteByte(0x01);
            packet.WriteByte(0x13);
            packet.WriteByte(0x01);
            packet.WriteByte(0x14);
            packet.WriteByte(0x01);
            packet.WriteByte(0x15);
            packet.WriteByte(Program.CheckSum8Xor(packet.ToArray().Skip(1).ToArray(), packet.ToArray().Skip(1).ToArray().Length));
            packet.WriteByte(0xc0);
            return packet;
        }
    }
}
