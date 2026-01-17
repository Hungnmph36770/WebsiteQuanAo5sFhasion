using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.VisualStudio.Web.CodeGeneration.Templating;
using Website.Models;
using Website.Areas.Admin.Attributes;
using Website.Models;
//sử dụng thư viện sau để phân trang
using X.PagedList;
using BC = BCrypt.Net;


namespace Website.Controllers
{
    public class ProductsController : Controller
    {
        public MyDbContext db = new MyDbContext();
        public IActionResult Category(int? id,int? page)
        {
            //xác định số trang hiện tại
            int page_number = page ?? 1;
            //số bản ghi trên một trang
            int page_size = 8;
            ViewBag.CategoryId = id;
            //lấy danh sách các bản ghi
            // Sai về xem lại
            //List<Products> listRecord = db.Products.ToList();
            List<Products> listRecord = (from p in db.Products
                                        join cp in db.categoriesProducts
                                            on p.Id equals cp.ProductId
                                        where cp.CategoryId == id
                                        select p).ToList();
            // Tạo 1 request để chọn
            string sapxep = HttpContext.Request.Query["sapxep"].ToString();
            switch (sapxep)
            {
                // Kết quả của biển listRecord có thể tiếp tục truy vấn
                // if chọn priceAsc ứng với price trong View thì listProduct sẽ sắp xếp
                case "priceAsc":
                    listRecord = listRecord.OrderBy(item => (item.Price - item.Discount)).ToList();
                    break;
                case "priceDesc":
                    listRecord = listRecord.OrderByDescending(item => (item.Price - item.Discount)).ToList();
                    break;
                case "nameAsc":
                    listRecord = listRecord.OrderBy(item => (item.Price - item.Discount)).ToList();
                    break;
                case "nameDesc":
                    listRecord = listRecord.OrderByDescending(item => (item.Price - item.Discount)).ToList();
                    break;
            }
            return View("ProductsCategory", listRecord.ToPagedList(page_number, page_size));
        }
        //chi tiết sản phẩm
        public IActionResult Detail(int id)
        {
            //lấy một bản ghi
            Products record = db.Products.FirstOrDefault(item => item.Id == id);
            var colors = db.Color.ToList();
            var sizes = db.Size.ToList();
            ViewBag.Color = colors;
            ViewBag.Size = sizes;
            return View("ProductDetail", record);
        }
        //đánh giá số sao của sản phẩm
        public IActionResult Rate(int id)
        {
            //lấy biến star truyền từ url
            int _Star = !String.IsNullOrEmpty(Request.Query["star"]) ? Convert.ToInt32(Request.Query["star"]) : 0;
            //thêm bản ghi vào table Rating
            Rating record = new Rating();
            record.ProductId = id;
            record.Star = _Star;
            db.Ratings.Add(record);
            db.SaveChanges();
            return Redirect("/Products/Detail/" + id);
        }
        public IActionResult Tk(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }
        // Tìm kiếm sản phẩm theo tên
        public IActionResult Search(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return View(new List<Products>());

            var result = db.Products
                .Where(p => p.Name.ToLower().Contains(key.ToLower()))
                .ToList();

            ViewBag.Key = key;
            return View(result);
        }


    }
}
