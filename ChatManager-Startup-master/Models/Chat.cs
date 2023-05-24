using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatManager.Models
{
    public class Chat
    {
        public Chat() {
            this.idAmi = tempRecievingId;
            this.idUser = tempRequestingId;
            this.message = tempMessage;
            messageSentTime = DateTime.Now;
        }
        public int Id { get; set; }
        public DateTime messageSentTime { get; set; }
        public int idAmi { get; set; }
        public int idUser { get; set; }
        public string message { get; set; }

        static public int tempRecievingId { get; set; }
        static public int tempRequestingId { get; set; }
        static public string tempMessage { get; set; }
    }
}