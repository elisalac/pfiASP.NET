using ChatManager.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ChatManager.Controllers
{
    public class ChatController : Controller
    {
        private int SelectedId
        {
            get
            {
                if (Session["SelectedId"] == null)
                {
                    Session["SelectedId"] = 0;
                }
                return (int)Session["SelectedId"];
            }
            set
            {
                Session["SelectedId"] = value;
            }
        }

        [OnlineUsers.UserAccess]
        public ActionResult Index()
        {
            OnlineUsers.GetSessionUser().AcceptNotification = true;
            return View();
        }
        [OnlineUsers.UserAccess(false)]
        public ActionResult GetFriendsList(bool forceRefresh = false)
        {
            if (forceRefresh || DB.Friendships.HasChanged)
            {
                List<Friendship> friendships = DB.Friendships.ToList().OrderBy(c => c.Id).Where(m => m.accepter && !m.refuser && m.requestingId == OnlineUsers.GetSessionUser().Id || m.accepter && !m.refuser && m.recievingId == OnlineUsers.GetSessionUser().Id).ToList();
                List<User> users = new List<User>();
                foreach (var friendship in friendships)
                {
                    if (friendship.recievingId == OnlineUsers.GetSessionUser().Id)
                        users.Add(DB.Users.Get(friendship.requestingId));
                    if (friendship.requestingId == OnlineUsers.GetSessionUser().Id)
                        users.Add(DB.Users.Get(friendship.recievingId));
                }
                return PartialView(users);
            }
            return null;
        }
        [OnlineUsers.UserAccess(false)]
        public ActionResult GetMessages(bool forceRefresh = false)
        {
            if (forceRefresh || DB.Chats.HasChanged)
            {
                ViewBag.SelectedId = SelectedId;
                return PartialView(Chats(SelectedId));
            }
            return null;
        }
        [OnlineUsers.UserAccess]
        public ActionResult SetCurrentTarget(int id)
        {
            SelectedId = id;
            return null;
        }

        public ActionResult Send(string message)
        {
            if (SelectedId != 0)
            {
                Chat.tempRequestingId = OnlineUsers.GetSessionUser().Id;
                Chat.tempRecievingId = SelectedId;
                Chat.tempMessage = message;
                Chat chat = new Chat();
                DB.Chats.Add(chat);
                OnlineUsers.AddNotification((int)Session["SelectedId"], $"Vous avez reÃ§u un message de {ChatManager.Models.OnlineUsers.GetSessionUser().GetFullName()}");
            }
            return null;
        }

        public ActionResult Delete(int id)
        {
            DB.Chats.Delete(id);
            return null;
        }

        public ActionResult Update(int id, string message)
        {
            Chat chat = DB.Chats.GetChat(id);
            chat.message = message;
            DB.Chats.Update(chat);

            return null;
        }
        public ActionResult DeleteChatLog(int id)
        {
            DB.Chats.Delete(id);
            return RedirectToAction("Chats");
        }
        [OnlineUsers.UserAccess(false)]
        public ActionResult GetChatLogs(bool forceRefresh = false)
        {
            if (forceRefresh || DB.Chats.HasChanged)
            {
                return PartialView(DB.Chats.ToList().OrderBy(c => c.messageSentTime));
            }
            return null;
        }

        [OnlineUsers.UserAccess]
        public ActionResult Chats()
        {
            return View();
        }

        private List<Chat> Chats(int userId)
        {

            return DB.Chats.ToList().OrderBy(c => c.messageSentTime).Where(c => c.idUser == OnlineUsers.GetSessionUser().Id || c.idAmi == OnlineUsers.GetSessionUser().Id && c.idAmi == userId || c.idUser == userId).ToList();
        }
    }
}