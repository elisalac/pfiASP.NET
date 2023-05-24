using ChatManager.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatManager.Controllers
{
    public class FriendshipsController : Controller
    {
        private string SearchText
        {
            get
            {
                if (Session["SearchText"] == null)
                {
                    Session["SearchText"] = "";
                }
                return (string)Session["SearchText"];
            }
            set
            {
                Session["SearchText"] = value;
            }
        }
        private bool FilterNotFriend
        {
            get
            {
                if (Session["FilterNotFriend"] == null)
                {
                    Session["FilterNotFriend"] = true;
                }
                return (bool)Session["FilterNotFriend"];
            }
            set
            {
                Session["FilterNotFriend"] = value;
            }
        }
        private bool FilterRequest
        {
            get
            {
                if (Session["FilterRequest"] == null)
                {
                    Session["FilterRequest"] = true;
                }
                return (bool)Session["FilterRequest"];
            }
            set
            {
                Session["FilterRequest"] = value;
            }
        }
        private bool FilterPending
        {
            get
            {
                if (Session["FilterPending"] == null)
                {
                    Session["FilterPending"] = true;
                }
                return (bool)Session["FilterPending"];
            }
            set
            {
                Session["FilterPending"] = value;
            }
        }
        private bool FilterFriend
        {
            get
            {
                if (Session["FilterFriend"] == null)
                {
                    Session["FilterFriend"] = true;
                }
                return (bool)Session["FilterFriend"];
            }
            set
            {
                Session["FilterFriend"] = value;
            }
        }
        private bool FilterRefused
        {
            get
            {
                if (Session["FilterRefused"] == null)
                {
                    Session["FilterRefused"] = true;
                }
                return (bool)Session["FilterRefused"];
            }
            set
            {
                Session["FilterRefused"] = value;
            }
        }
        private bool FilterBlocked
        {
            get
            {
                if (Session["FilterBlocked"] == null)
                {
                    Session["FilterBlocked"] = true;
                }
                return (bool)Session["FilterBlocked"];
            }
            set
            {
                Session["FilterBlocked"] = value;
            }
        }

        // GET: Friendships
        [OnlineUsers.UserAccess]
        public ActionResult Index()
        {
            OnlineUsers.GetSessionUser().AcceptNotification = true;
            ViewBag.FilterPending = FilterPending;
            ViewBag.FilterFriend = FilterFriend;
            ViewBag.FilterNotFriend = FilterNotFriend;
            ViewBag.FilterBlocked = FilterBlocked;
            ViewBag.FilterRefused = FilterRefused;
            ViewBag.FilterRequest = FilterRequest;
            return View();
        }

        public ActionResult GetFriendShipsStatus(bool forceRefresh = false)
        {
            if (forceRefresh || DB.Friendships.HasChanged)
            {
                return PartialView(Users());
            }

            return null;
        }
        private List<User> Users()
        {
            int sessionId = OnlineUsers.GetSessionUser().Id;
            List<User> users = new List<User>();
            foreach (User user in DB.Users.ToList())
            {
                if (FilterFriend)
                {

                    if (DB.Friendships.ToList().Where(i => i.accepter && !i.refuser && user.Id == i.recievingId && i.requestingId == sessionId).FirstOrDefault() != null || DB.Friendships.ToList().Where(i => i.accepter && !i.refuser && user.Id == i.requestingId && i.recievingId == sessionId).FirstOrDefault() != null)
                    {
                        users.Add(user);
                    }

                }
                if (FilterRefused)
                {

                    if (DB.Friendships.ToList().Where(i => !i.accepter && i.refuser && user.Id == i.recievingId && i.requestingId == sessionId).FirstOrDefault() != null || DB.Friendships.ToList().Where(i => !i.accepter && i.refuser && user.Id == i.requestingId && i.recievingId == sessionId).FirstOrDefault() != null)
                    {
                        users.Add(user);
                    }

                }
                if (FilterPending)
                {
                    if (DB.Friendships.ToList().Where(i => i.requestingId == OnlineUsers.GetSessionUser().Id && i.recievingId == user.Id && i.accepter && i.refuser).FirstOrDefault() != null)
                    {
                        users.Add(user);
                    }
                }
                if (FilterRequest)
                {
                    if (DB.Friendships.ToList().Where(i => i.recievingId == OnlineUsers.GetSessionUser().Id && i.requestingId == user.Id && i.accepter && i.refuser).FirstOrDefault() != null)
                    {

                        users.Add(user);

                    }
                }
                if (FilterNotFriend)
                {


                    if (DB.Friendships.ToList().Where(i => user.Id == i.recievingId && i.requestingId == sessionId || user.Id == i.requestingId && i.recievingId == sessionId).FirstOrDefault() == null && !user.Blocked && user.Id != sessionId)
                    {
                        users.Add(user);
                    }

                }

            }
            if (FilterBlocked)
            {
                List<User> userBlocked = DB.Users.ToList().Where(u => u.Blocked == true).ToList();
                users.AddRange(userBlocked);
            }
            if (SearchText != "")
            {
                users = users.Where(u => u.FirstName.ToLower().Contains(SearchText.ToLower()) || u.LastName.ToLower().Contains(SearchText.ToLower())).ToList();
            }
            return users.OrderBy(u => u.GetFullName()).ToList();

        }
        public ActionResult SendFriendshipRequest(int id)
        {
            Friendship friendship = DB.Friendships.GetFriendship(id, OnlineUsers.GetSessionUser().Id);
            if (friendship == null)
            {
                Friendship.tempRequestingId = OnlineUsers.GetSessionUser().Id;
                Friendship.tempRecievingId = id;
                Friendship newFriendship = new Friendship();
                DB.Friendships.Add(newFriendship);
            }
            else
            {
                DB.Friendships.Delete(friendship.Id);
                Friendship.tempRequestingId = OnlineUsers.GetSessionUser().Id;
                Friendship.tempRecievingId = id;
                Friendship newFriendship = new Friendship();
                DB.Friendships.Add(newFriendship);
            }
            OnlineUsers.AddNotification(id, $"Vous avez reçu une demande d'ami de {ChatManager.Models.OnlineUsers.GetSessionUser().GetFullName()}");
            return RedirectToAction("Index");
        }

        public ActionResult RemoveFriendshipRequest(int id)
        {
            Friendship friendship = DB.Friendships.GetFriendship(id, OnlineUsers.GetSessionUser().Id);
            DB.Friendships.Delete(friendship.Id);
            OnlineUsers.AddNotification(id, $"{ChatManager.Models.OnlineUsers.GetSessionUser().GetFullName()} a enlevé votre amitié");
            return RedirectToAction("Index");
        }
        public ActionResult AcceptFriendshipRequest(int id)
        {
            Friendship friendship = DB.Friendships.GetFriendship(id, OnlineUsers.GetSessionUser().Id);
            friendship.accepter = true;
            friendship.refuser = false;
            DB.Friendships.Update(friendship);
            OnlineUsers.AddNotification(id, $"{ChatManager.Models.OnlineUsers.GetSessionUser().GetFullName()} a accepté votre demande d'amitié");
            return RedirectToAction("Index");
        }
        public ActionResult DeclineFriendshipRequest(int id)
        {
            Friendship friendship = DB.Friendships.GetFriendship(OnlineUsers.GetSessionUser().Id, id);
            friendship.accepter = false;
            friendship.refuser = true;
            DB.Friendships.Update(friendship);
            OnlineUsers.AddNotification(id, $"{ChatManager.Models.OnlineUsers.GetSessionUser().GetFullName()} a refusé votre demande d'amitié");
            return RedirectToAction("Index");
        }
        public ActionResult SetFilterRequest(bool check)
        {
            if (check)
            {
                FilterRequest = true;
            }
            else
            {
                FilterRequest = false;
            }
            return null;
        }
        public ActionResult SetFilterFriend(bool check)
        {
            if (check)
            {
                FilterFriend = true;
            }
            else
            {
                FilterFriend = false;
            }
            return null;
        }

        public ActionResult SetFilterNotFriend(bool check)
        {
            if (check)
            {
                FilterNotFriend = true;
            }
            else
            {
                FilterNotFriend = false;
            }
            return null;
        }


        public ActionResult SetFilterPending(bool check)
        {
            if (check)
            {
                FilterPending = true;
            }
            else
            {
                FilterPending = false;
            }
            return null;
        }
        public ActionResult SetFilterRefused(bool check)
        {
            if (check)
            {
                FilterRefused = true;
            }
            else
            {
                FilterRefused = false;
            }
            return null;
        }

        public ActionResult SetFilterBlocked(bool check)
        {
            if (check)
            {
                FilterBlocked = true;
            }
            else
            {
                FilterBlocked = false;
            }
            return null;
        }
        public ActionResult Search(string text)
        {
            SearchText = text;

            return null;
        }

    }

}