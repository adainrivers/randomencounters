using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BloodyEncounters.Exceptions
{
    internal class ProductDontExistException : Exception
    {
        public ProductDontExistException()
        {
        }

        public ProductDontExistException(string message)
            : base(message)
        {
        }

        public ProductDontExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ProductDontExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
