
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using SearchRelease.DB;

namespace SearchBate.Controllers
{
    public class LoginController : Controller
    {
        private readonly myDB _db = new myDB();

        // GET: Login
        
        [AllowAnonymous]
        public ActionResult Login(string Account = null,string Pwd = null)
        { 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Check(string Account = null, string Pwd = null)
        {
            if (!string.IsNullOrEmpty(Pwd))
            {
                byte[] BytePwa = Convert.FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes(Pwd)));
                var SaltPwd = AddSaltToPwd(BytePwa);
                var Qeury = _db.Account.Where(o => o.Account1 == Account && o.Pwd == SaltPwd);
                if (Qeury.Any())
                {
                    var AuthManager = Request.GetOwinContext().Authentication;
                    var Identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Role, "Administrator")
                    }, "Ksu");
                    AuthManager.SignIn(Identity);
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Login","Login");
        }
        private string AddSaltToPwd(Byte[] Pwd)
        {
            //位元組的隨機金鑰
            int Byte_cb = 8;

            //定義兩組SaltKey,必須大於8Byte
            byte[] SaltKey_1 = Encoding.UTF8.GetBytes("ABCDAAAA");
            byte[] SaltKey_2 = Encoding.UTF8.GetBytes("@ABCDABCD");

            //First Add Salt
            Rfc2898DeriveBytes Rfc_1 = new Rfc2898DeriveBytes(Pwd, SaltKey_1, 2);
            byte[] GetRfc_1 = Rfc_1.GetBytes(Byte_cb);
            var Salt_1Pwd = Convert.ToBase64String(GetRfc_1);

            //Second Add Salt
            Rfc2898DeriveBytes Rfc_2 = new Rfc2898DeriveBytes(Salt_1Pwd, SaltKey_2);
            byte[] GetRfc_2 = Rfc_2.GetBytes(Byte_cb);

            var Salt_2Pwd = Convert.ToBase64String(GetRfc_2);
            return Salt_2Pwd;
        }
    }
}