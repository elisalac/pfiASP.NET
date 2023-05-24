using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class Friendship
    {
        public Friendship()
        {
            this.requestingId = tempRequestingId;
            this.recievingId = tempRecievingId;
            accepter = true;
            refuser = true;
            CreationDate = DateTime.Now;
        }
        public Friendship Clone()
        {
            return JsonConvert.DeserializeObject<Friendship>(JsonConvert.SerializeObject(this));
        }
        public int Id { get; set; }
        public int requestingId { get; set; }
        public int recievingId { get; set; }
        public DateTime CreationDate { get; set; }
        public bool accepter { get; set; }
        public bool refuser { get; set; }
        static public int tempRecievingId { get; set; }
        static public int tempRequestingId { get; set; }


        /*
         Pending//accepter:true  refuser:true
         Refuser//accepter:false  refuser:true
         Amis//accepter:true   refuser:false
         Possible friend//accepter:false refuser:false
         */

        //[JsonIgnore]
        //public List<User> Users { get { return DB.Users.ToList().Where(u => u.Id == Id).ToList(); } }

        //[JsonIgnore]
        //public List<Friendship> Friendships
        //{
        //    get
        //    {
        //        var friendships = new List<Friendship>();
        //        foreach (var user in Users.OrderBy(u => u.))

        //        return friendships;
        //    }
        //}
    }

}