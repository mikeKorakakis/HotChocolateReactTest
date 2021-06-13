

using System;

namespace API
{
    public class Message
    {
        public virtual Guid Id { get; set; }
        public string Body { get; set; }

        public DateTime CreatedAt { get; set; }
    }

}