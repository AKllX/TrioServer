using System;
using System.Collections.Generic;
using System.Text;

namespace TrioServer.Communication
{
    public class RadioMessage
    {
        private byte[] mBody;
        
        private int mPointer;
        public int RadioSerialNumber { get; private set; }

        public byte[] FullPacket { get; private set; }
        public int Length
        {
            get
            {
                return mBody.Length;
            }
        }

        public int RemainingLength
        {
            get
            {
                return mBody.Length - mPointer;
            }
        }

        public byte[] ReadBytes(int Bytes)
        {
            if (Bytes > this.RemainingLength)
            {
                Bytes = this.RemainingLength;
            }

            byte[] data = new byte[Bytes];

            for (int i = 0; i < Bytes; i++)
            {
                data[i] = mBody[mPointer++];
            }

            return data;
        }

        public Int32 GetInt32()
        {
            return Convert.ToInt32(ReadBytes(4));
        }

        public byte GetByte()
        {
            return ReadBytes(1)[0];
        }

        public UInt16 GetUInt16()
        {
            return Convert.ToUInt16(ReadBytes(2));
        }

        public short GetSignedInt16()
        {
            byte[] test = ReadBytes(2);
            Array.Reverse(test);

            return BitConverter.ToInt16(test,0);
        }

        public RadioMessage(int radioSn, byte[] message)
        {
            this.RadioSerialNumber = radioSn;
            mBody = message;
            FullPacket = message;
        }
    }
}
