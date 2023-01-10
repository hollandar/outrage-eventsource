using System.Runtime.Serialization;

namespace Outrage.EventSource.File
{
    [Serializable]
    internal class EventReadException : Exception
    {
        public EventReadException()
        {
        }

        public EventReadException(string? message) : base(message)
        {
        }

        public EventReadException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected EventReadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}