using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMCS_LHM
{
    class help_functions
    {
        public string ByteArrayToString(byte[] bytes)
        {
            string outstr = "";
            Encoding ascii = Encoding.ASCII;
            outstr = new string(ascii.GetChars(bytes));

            return outstr;
        }

        public byte[] StringToByteArray(string str, int length)
        {
            byte[] outarr = new byte[length];
            Encoding ascii = Encoding.ASCII;
            byte[] chars = ascii.GetBytes(str);
            Array.Copy(chars, 0, outarr, 0, str.Length);
            return outarr;
        }

        public int ByteArrayLEToInt(byte[] bytearr)
        {
            Array.Reverse(bytearr);
            int outint = BitConverter.ToInt32(bytearr, 0);

            return outint;
        }

        public byte[] IntToByteArrayLE(int number)
        {
            byte[] outarr = new byte[4];
            outarr = BitConverter.GetBytes(number);
            Array.Reverse(outarr);

            return outarr;
        }

        public byte[] spliceByteArray(byte[] inbytearr, ref byte[] outbytearr, int offset, int length)
        {
            Array.Resize(ref outbytearr, length);
            Array.Copy(inbytearr, offset, outbytearr, 0, length);
            return outbytearr;
        }
    }
}
