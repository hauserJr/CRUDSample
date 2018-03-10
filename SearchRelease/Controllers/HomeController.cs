using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SearchRelease.DB;
using System.Web.Security;
using System.Text;
using System.Security.Cryptography;

namespace SearchRelease.Controllers
{
    public class HomeController : Controller
    {
        private readonly myDB _db = new myDB();

        [Authorize]
        public ActionResult Index()
        {
            var x = User.Identity.Name;
            return View();
        }

        #region CRUD
        [HttpPost]
        public string SelectData(string id)
        {
            Product _Product = new Product();
            var query = _db.KSU_Product
                .Where(o => o.ProductName.Contains(id.TrimEnd()))
                .Select(o => new Product()
                {
                    ProductName = o.ProductName
                    ,
                    ProductArea = o.ProductArea
                    ,
                    CompanyName = o.CompanyName
                    ,
                    id = o.Id
                }).ToList();
            string json = JsonConvert.SerializeObject(query);
            return json;
        }
        [HttpPost]
        public string SingleData(int id)
        {
            Product _Product = new Product();
            var query = _db.KSU_Product
                .Where(o => o.Id == id)
                .Select(o => new Product()
                {
                    ProductName = o.ProductName
                    ,
                    ProductArea = o.ProductArea
                    ,
                    CompanyName = o.CompanyName
                    ,
                    id = o.Id
                }).ToList();
            string json = JsonConvert.SerializeObject(query);
            return json;
        }
        [HttpPost]
        public string Del(int id,string Pwd = null)
        {
            if (!string.IsNullOrEmpty(Pwd))
            {
                byte[] BytePwa = Convert.FromBase64String(Convert.ToBase64String(Encoding.UTF8.GetBytes(Pwd)));
                var SaltPwd = AddSaltToPwd(BytePwa);
                var Qeury = _db.Account.Where(o => o.Pwd == SaltPwd);
                if (Qeury.Any())
                {
                    Product _Product = new Product();
                    var query = _db.KSU_Product
                        .Where(o => o.Id == id)
                        .FirstOrDefault();
                    _db.KSU_Product.Remove(query);
                    _db.SaveChanges();
                    return "200";
                }
            }
            return "";
        }

        [HttpPost]
        public string Create(Product _Product)
        {
            _db.KSU_Product.Add(new KSU_Product()
            {
                ProductName = _Product.ProductName,
                ProductArea = _Product.ProductArea,
                CompanyName = _Product.CompanyName
            });
            _db.SaveChanges();
            return "200";
        }

        [HttpPost]
        public string SaveEdit(Product _Product)
        {
            var query = _db.KSU_Product.Where(o => o.Id == _Product.id);
            if (query.Any())
            {
                var QueryData = query.FirstOrDefault();
                QueryData.ProductName = _Product.ProductName;
                QueryData.ProductArea = _Product.ProductArea;
                QueryData.CompanyName = _Product.CompanyName;
                _db.SaveChanges();

            }
            return "200";
        }
        #endregion

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