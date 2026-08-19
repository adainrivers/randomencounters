using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BloodyEncounters.Exceptions
{
    internal class NPCDontExistException : Exception
    {
        public NPCDontExistException()
        {
        }

        public NPCDontExistException(string message)
            : base(message)
        {
        }

        public NPCDontExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NPCDontExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
