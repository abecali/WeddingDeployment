using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wedding_planner.Models;

namespace wedding_planner.Controllers
{
    public class HomeController : Controller
    {
        // this the datebase connection
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        // SESSION
        private int? UserSession
        {
            get { return HttpContext.Session.GetInt32("UserId"); }
            set { HttpContext.Session.SetInt32("UserId", (int)value); }
        }


        //  this is the index page where it checks id user in session is logged in if not redirected to index ( loging and reg page)
        public IActionResult Index()
        {
            if (UserSession != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        // this is logout function ( method)
        [HttpGet("/logout")]
        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }


        // this is Registration Method 
        [HttpPost("/new/Users")]
        public IActionResult Registration(User newUser)
        {
            if (ModelState.IsValid)
            {
                // if email provided is NOT in database create new account
                if (dbContext.Users.FirstOrDefault(user => user.Email == newUser.Email) == null)
                {
                    PasswordHasher<User> hasher = new PasswordHasher<User>();
                    newUser.Password = hasher.HashPassword(newUser, newUser.Password);
                    dbContext.Add(newUser);
                    dbContext.SaveChanges();
                    UserSession = newUser.UserId;

                    return RedirectToAction("Dashboard");

                }
                else
                {
                    // if it already exist say to log in
                    ModelState.AddModelError("email", "Please log in");
                    return View("Index");
                }
            }
            return View("Index");
        }


        //Login method 
        [HttpPost("User/login")]
        public IActionResult Login(Login userinfo)
        {
            if (ModelState.IsValid)
            {
                User userFromDb = dbContext.Users.FirstOrDefault(user => user.Email == userinfo.LoginEmail);
                if (userFromDb == null)
                {
                    // Create error if email not in db
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }
                var hasher = new PasswordHasher<Login>();
                var result = hasher.VerifyHashedPassword(userinfo, userFromDb.Password, userinfo.LoginPassword);

                if (result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid cradentails");
                    return View("Index");

                }
                // this is grabbing the Id of user from Db and assing it to UserSession. so user session is ID number not other fields
                UserSession = userFromDb.UserId;
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

        // this is to just to load dashboard with information from DB
        [HttpGet("/dashboard")]
        public IActionResult Dashboard()
        {
            if (UserSession == null)
                return RedirectToAction("Index");
            // User currentUser = dbContext.Users.Include(user => user.WeddingsCreated).ThenInclude(user => user.Associations).FirstOrDefault(user => user.UserId == userId);
            // if (UserSession != userId)
            // {
            //     return RedirectToAction("Dashboard", new { userId = UserSession });
            // }
            ViewBag.userId = UserSession;
            List<Wedding> AllWeddings = dbContext.Weddings
                .Include(w => w.Associations)
                .OrderByDescending(w => w.Date)
                .ToList();
            return View(AllWeddings);
        }


        // this is just to load new weeding page to add wedding
        [HttpGet("Wedding/New")]
        public IActionResult NewWeddingPage()
        {
            return View();
        }

        // this is for Wedding Info page load
        [HttpGet("Wedding/Info/{WeddingIDfromAddress}")]
        public IActionResult WeddingInfo(int WeddingIDfromAddress)

        {
            if (UserSession == null)
            {
                return RedirectToAction("Index");
            }
            Wedding weddingdetails = dbContext.Weddings.Include(asso => asso.Associations).ThenInclude(x => x.Guest).FirstOrDefault(w => w.WeddingId == WeddingIDfromAddress);



            return View(weddingdetails);

        }


        // this is to add new add with Post method to database
        [HttpPost("Wedding/New")]
        public IActionResult CreateWedding(Wedding newWedding)
        {
            // next two lines checking if the User loggin by checking sesison
            if (UserSession == null)
                return RedirectToAction("Index");
            if (ModelState.IsValid)
            {
                // Create new wedding with UserID set to current session user's ID
                newWedding.UserId = (int)UserSession;
                dbContext.Weddings.Add(newWedding);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("NewWeddingPage");
            }
        }


        // delete button( method )
        [HttpGet("Wedding/Delete/{WeddingIDfromAddress}")]
        public IActionResult Delete(int WeddingIDfromAddress)
        {
            Wedding WeddingToDelete = dbContext.Weddings.FirstOrDefault(w => w.WeddingId == WeddingIDfromAddress);
            dbContext.Weddings.Remove(WeddingToDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }



        // this is to RSVP button ( method)
        [HttpGet("Wedding/RSVP/{WeddingIDfromAddress}")]
        public IActionResult RSVP(int WeddingIDfromAddress)
        {
            if (UserSession == null)
                return RedirectToAction("Index");
            Association going = new Association();
            going.UserId = (int)UserSession;
            going.WeddingId = WeddingIDfromAddress;
            dbContext.Add(going);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }


        [HttpGet("Wedding/UNRSVP/not-going/{WeddingIDfromAddress}")]
        public IActionResult UnRSVP(int WeddingIDfromAddress)
        {
            if (UserSession == null)
                return RedirectToAction("Index");
            Association notGoing = dbContext.Associations.FirstOrDefault(asso => asso.WeddingId == WeddingIDfromAddress && asso.UserId == UserSession);
            dbContext.Associations.Remove(notGoing);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }


        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

























