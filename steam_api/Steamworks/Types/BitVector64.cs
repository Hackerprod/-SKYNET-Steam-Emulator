using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Steamworks.Types
{
    public class BitVector64
    {
        private UInt64 data;

        public BitVector64()
        {
        }
        public BitVector64(UInt64 value)
        {
            data = value;
        }

        public UInt64 Data
        {
            get { return data; }
            set { data = value; }
        }

        public UInt64 this[uint bitoffset, UInt64 valuemask]
        {
            get
            {
                return (data >> (ushort)bitoffset) & valuemask;
            }
            set
            {
                data = (data & ~(valuemask << (ushort)bitoffset)) | ((value & valuemask) << (ushort)bitoffset);
            }
        }
    }
}
