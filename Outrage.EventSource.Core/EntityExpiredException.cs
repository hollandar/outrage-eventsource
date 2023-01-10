using System.Runtime.Serialization;

namespace Outrage.EventSource.Core
{
    [Serializable]
    internal class EntityExpiredException : Exception
    {
        public EntityExpiredException()
        {
        }

        public EntityExpiredException(string? message) : base(message)
        {
        }

        public EntityExpiredException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected EntityExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}