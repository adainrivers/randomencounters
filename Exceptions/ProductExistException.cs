using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BloodyEncounters.Exceptions
{
    internal class ProductExistException : Exception
    {
        public ProductExistException()
        {
        }

        public ProductExistException(string message)
            : base(message)
        {
        }

        public ProductExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ProductExistException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
