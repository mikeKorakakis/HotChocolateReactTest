using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.AspNetCore.Identity;

namespace API
{


    [ExtendObjectType(Name = "Query")]

    public class UserQueries
    {
       

        [UseApplicationDbContext]
        public async Task<IEnumerable<AppUser>> GetUsers(
               [ScopedService] DataContext context,
               [Service] UserManager<AppUser> userManager,
               UserByIdDataLoader userById,
               CancellationToken cancellationToken
               )
        {
            
              string[] userIds = {
                "be5cgf48-7f69-4a75-8de8-076e570bade5",
                "38f2h820-f93f-4948-92b6-bdc283a351f1",
                "5ae57946-8eab-42ab-8054-099b8f79c37a",
            };
            var users = await userById.LoadAsync(userIds, cancellationToken);

            return users;
        }

       
    }

}