using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
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
        [Authorize]
        [Subscribe(With = nameof(SubscribeToOnSendMessageAsync))]
        public Task<Message> onSendMessage(
            [Service] ITopicEventReceiver eventReceiver,
            [EventMessage] Message message,
        [Service] IUserAccessor userAccessor,
            [Service] ILogger<MessageSubscriptions> logger
            )
        {
            var userId = userAccessor.GetCurrentUsername();
            var newMessage = new Message { Id = message.Id, Body = $"{message.Body} sent by {userId}", CreatedAt = message.CreatedAt };
            return Task.FromResult(newMessage);
        }


        public async ValueTask<ISourceStream<Message>> SubscribeToOnSendMessageAsync(
        [Service] ITopicEventReceiver eventReceiver,
        CancellationToken cancellationToken)
        {
            return await eventReceiver.SubscribeAsync<string, Message>(
                 $"send_message", cancellationToken);
        }
    }


}
