using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UntitledChatApp
{
    public struct Id64
    {
        ulong backingVal;

        public static implicit operator ulong(Id64 id)
        {
            return id.backingVal;
        }


    }
}