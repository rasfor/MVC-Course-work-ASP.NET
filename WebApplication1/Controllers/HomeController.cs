using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Windows.Forms;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        MyContext db = new MyContext();
      
       

        public ActionResult Index(string searchString)
        {

           
            var orders = from m in db.Orders
                         select m;

           
            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(s => s.name.Contains(searchString));
            }

            return View(orders);
        }

        /*public ActionResult Index()
        {
            return View(db.Orders);
        }
       */
        [HttpGet]
        public ActionResult MyAccount(int? id)
        {
            
            if (id == null)
            {
                return HttpNotFound();
            }
            User user = db.Users.Find(id);
            if (user != null)
            {
                return View(user);
            }
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult MyAccount(User user)
        {
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Order order)
        {
            db.Orders.Add(order);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        public ActionResult Mypage(User user)
        {
            
            return View(user);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User model)
        {
            if (ModelState.IsValid)
            {
                // поиск пользователя в бд
                User user = null;
                using (MyContext db = new MyContext())
                {
                    user = db.Users.FirstOrDefault(u => u.email == model.name && u.password == model.password);

                }
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.name, true);
                    //MembershipUser u = Membership.GetUser(user.name);                  
                    return RedirectToAction("Mypage", user);
                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
                }
            }
        
            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                using (MyContext db = new MyContext())
                {
                    user = db.Users.FirstOrDefault(u => u.email == model.name);
                }
                if (user == null)
                {
                    // создаем нового пользователя
                    using (MyContext db = new MyContext())
                    {
                        db.Users.Add(new User { login = model.login, password = model.password, name = model.name, email = model.email }) ;
                        db.SaveChanges();
                        MessageBox.Show("Вы успешно зарегистрированы!", "Регистрация", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        user = db.Users.Where(u => u.email == model.name && u.password == model.password).FirstOrDefault();
                    }
                    // если пользователь удачно добавлен в бд
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.name, true);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                }
            }

            return View(model);
        }
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult MyList(User model)
        {
            var orders = from m in db.Orders
                         select m;

                orders = orders.Where(s => s.user_id==model.Id);
            
            return View(orders);
        }


        public ActionResult AllOrders(string searchString, int id)
        {
            var orders = from m in db.Orders
                         select m;
            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(s => s.name.Contains(searchString));
            }
            orders = orders.Where(s => s.user_id ==  null);
            TempData["Message"] = id;
            return View(orders);
        }

        public ActionResult Choose(int id)
        {
            ViewBag.Message = TempData["Message"];
            int id_ = ViewBag.Message;
            Order order = db.Orders.Find(id);
            if (order.user_id == null)
            {
                order.user_id = id_;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return View();
            }
            else
                return  View("ChooseError");
        }

        public ActionResult Delete(int id)
        {
           Order b = db.Orders.Find(id);
            if (b != null)
            {
                db.Orders.Remove(b);
                db.SaveChanges();
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
