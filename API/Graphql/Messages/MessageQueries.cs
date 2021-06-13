using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;

namespace API
{
    [ExtendObjectType(Name = "Query")]

    public class MessageQueries
    {      

        [UseApplicationDbContext]
        public  List<Message> GetMessages(
            [ScopedService] DataContext context
        )
        {           
            var messages = context.Messages.OrderBy(m => m.CreatedAt).ToList();
            return messages;
        }


    }

}