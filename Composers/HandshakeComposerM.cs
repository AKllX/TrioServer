using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

using TrioServer.Sessions;

namespace TrioServer.Composers
{
    class HandshakeComposerM
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
            packet.WriteByte(0x4c); //DEF Rádio M
            //packet.WriteByte(mSession.PacketTick()); Não há packet counter
            packet.WriteByte(0x00);
            packet.WriteByte(0x62);
            packet.WriteByte(0x00);
            packet.WriteByte(0xc6);
            packet.WriteByte(0x00);
            packet.WriteByte(0xbe);
            packet.WriteByte(0x00);
            packet.WriteByte(0x03);
            packet.WriteByte(0x00);
            packet.WriteByte(0x06);
            packet.WriteByte(0x00);
            packet.WriteByte(0x09);
            packet.WriteByte(0x00);
            packet.WriteByte(0x0f);
            packet.WriteByte(0x00);
            packet.WriteByte(0x0c);
            packet.WriteByte(0x00);
            packet.WriteByte(0x12);
            packet.WriteByte(Program.CheckSum8Xor(packet.ToArray().Skip(1).ToArray(), packet.ToArray().Skip(1).ToArray().Length));
            packet.WriteByte(0xc0);
            return packet;
        }
    }
}
