using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API
{
    [ExtendObjectType(Name = "Mutation")]
    public class MessageMutations
    {
        public record CreateMessageInput(
            string Body
        );


        [UseApplicationDbContext]
        public async Task<Message> CreateMessage(
            CreateMessageInput input,
            [ScopedService] DataContext context,
            // [Service] ITopicEventSender eventSender,
            // [Service] ILogger<MessageMutations> logger,
            CancellationToken cancellationToken
            )
        {


            var message = new Message
            {
                Body = input.Body,
                CreatedAt = DateTime.Now
            };


            context.Messages.Add(message);
            var success = await context.SaveChangesAsync() > 0;

            if (!success)
            {
                throw new QueryException("Problem saving changes");
            }


            // await eventSender.SendAsync($"send_message", message, cancellationToken);
            return message;

        }
    }
}