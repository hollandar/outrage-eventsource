using System.Runtime.Serialization;

namespace Outrage.EventSource.Core
{
    [Serializable]
    internal class EntityServiceException : Exception
    {
        public EntityServiceException()
        {
        }

        public EntityServiceException(string? message) : base(message)
        {
        }

        public EntityServiceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected EntityServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}