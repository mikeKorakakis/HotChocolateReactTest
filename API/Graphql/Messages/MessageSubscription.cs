using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Microsoft.Extensions.Logging;

namespace API
{
    [ExtendObjectType(Name = "Subscription")]
    public class MessageSubscriptions
    {

        // [Topic]
        // [Subscribe]
        [Subscribe(With = nameof(SubscribeToOnSendMessageAsync))]
        public Task<Message> onSendMessage(
            [Service] ITopicEventReceiver eventReceiver,
            [EventMessage] Message message,
            [Service] ILogger<MessageSubscriptions> logger
            ) => Task.FromResult(message);


        public async ValueTask<ISourceStream<Message>> SubscribeToOnSendMessageAsync(
        [Service] ITopicEventReceiver eventReceiver,
        [Service] IUserAccessor userAccessor,
        CancellationToken cancellationToken)
        {
            var userId = userAccessor.GetCurrentUsername();
            return await eventReceiver.SubscribeAsync<string, Message>(
                 $"send_message_{userId}", cancellationToken);
        }
    }


}
