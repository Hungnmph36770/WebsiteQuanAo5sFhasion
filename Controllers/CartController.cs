using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
// Bắt buộc
using Newtonsoft.Json;
using Website.Areas.Admin.Attributes;
using Website.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Website.Models;
using X.PagedList;
namespace Website.Controllers
{
    public class CartController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Index()
        {
            // Lấy chuỗi json 
            string json_cart = HttpContext.Session.GetString("cart");
            // Tạo biến Cart để đổ dữ liệu từ biến json vào 
            List<Item> cart = new List<Item>();
            if (!String.IsNullOrEmpty(json_cart))
            {
                // Chuyển json ra dạng list
                cart = JsonConvert.DeserializeObject<List<Item>>(json_cart);
            }
            return View("Index", cart);
        }
        // Cho sản phẩm vào giỏ hàng
        public IActionResult Buy(int id)
        {
            // Gọi hàm Add từ class Cart
            Cart.CartAdd(HttpContext.Session, id);
            return RedirectToAction("Index");

        }
        // Xoá sản phẩm khỏi giỏ hàng
        public IActionResult Remove(int id)
        {
            // Gọi hàm Remove từ class Cart
            Cart.CartRemove(HttpContext.Session, id);
            return RedirectToAction("Index");
        }
        // Cập nhật số lượng sản phẩm
        [HttpPost]
        public IActionResult Update()
        {
            // Lấy chuỗi json 
            string json_cart = HttpContext.Session.GetString("cart");
            // Tạo biến Cart để đổ dữ liệu từ biến json vào 
            List<Item> cart = new List<Item>();
            if (!String.IsNullOrEmpty(json_cart))
            {
                // Chuyển json ra dạng list
                cart = JsonConvert.DeserializeObject<List<Item>>(json_cart);
            }
            // Duyệt các Item trong list cart để Update số lượng
            foreach (var product in cart)
            {
                int quantity = Convert.ToInt32(Request.Form["product_" + product.ProductRecord.Id]);
                // Gọi hàm Cart Update để Update số lượng
                Cart.CartUpdate(HttpContext.Session, product.ProductRecord.Id, quantity);
            }
            return Redirect("/Cart");
        }
        // Xoá toàn bộ sản phẩm trong giỏ hàng
        public IActionResult Destroy(int id)
        {
            Cart.CartDestroy(HttpContext.Session);
            return Redirect("/Cart");
        }
        // Thanh toán giỏ hàng
        public IActionResult Checkout()
        {
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("customer_user_email")))
            {
                Cart.CartCheckOut(HttpContext.Session, Convert.ToInt32(HttpContext.Session.GetString("customer_user_id")));
                return Redirect("/Cart");
            }
            else

                return Redirect("/Account/Login");

            return Redirect("/Cart");
        }
    }
}
