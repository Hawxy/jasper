using Jasper.Transports;
using Jasper.Transports.Sending;

namespace Jasper.Runtime.Scheduled
{
    public static class EnvelopeScheduleExtensions
    {
        public static Envelope ForScheduledSend(this Envelope envelope, ISendingAgent sender)
        {
            return new Envelope(envelope, EnvelopeReaderWriter.Instance)
            {
                Message = envelope,
                MessageType = TransportConstants.ScheduledEnvelope,
                ExecutionTime = envelope.ExecutionTime,
                ContentType = TransportConstants.SerializedEnvelope,
                Destination = TransportConstants.DurableLocalUri,
                Status = EnvelopeStatus.Scheduled,
                OwnerId = TransportConstants.AnyNode,
                Sender = sender,
                Data = envelope.Serialize()
            };
        }
    }
}
