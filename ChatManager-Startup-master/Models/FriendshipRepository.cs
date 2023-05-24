using Antlr.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class FriendshipRepository : Repository<Friendship>
    {
        public List<User> users;

        public override int Add(Friendship friendship)
        {
            BeginTransaction();
            base.Add(friendship);
            EndTransaction();
            return friendship.Id;
        }


        public override bool Update(Friendship friendship)
        {
            BeginTransaction();
            var result = base.Update(friendship);
            EndTransaction();
            return result;
        }
        public override bool Delete(int Id)
        {
            Friendship friendship = DB.Friendships.Get(Id);
            if (friendship != null)
            {
                BeginTransaction();
                var result = base.Delete(Id);
                EndTransaction();
                return result;
            }
            return false;
        }
        public Friendship GetCurrentUserFriendshipStatus(int userId)
        {
            User user = DB.Users.Get(userId);
            if (user != null)
            {
                Friendship friendship = DB.Friendships.ToList().Where(u => u.requestingId == userId).FirstOrDefault();
                return friendship;
            }
            return null;
        }
        public Friendship GetFriendship(int recievingId, int requestingId)
        {
            Friendship friendship = DB.Friendships.ToList().Where(i => i.recievingId == recievingId && i.requestingId == requestingId || i.requestingId == recievingId && i.recievingId == requestingId).FirstOrDefault();
            if (friendship != null)
            {
                return friendship;
            }
            return null;
        }
    }
}