using BusinessLayer.Concrete;
using BusinessLayer.ValidationRules;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace KonusarakOgrenWebApp.Controllers
{
    public class ProductController : Controller
    {
        ProductManager pm = new ProductManager(new EfProductRepository());
        Context c = new Context();
        public IActionResult Index()
        {
            var values = pm.GetList();
            return View(values);
        }
        [HttpGet]
        public IActionResult ProductAdd()
        {
            return View();

        }
        [HttpPost]
        public IActionResult ProductAdd(Product product)
        {
            ProductValidator bv = new ProductValidator();
            ValidationResult result = bv.Validate(product);
            if (result.IsValid)
            {
                product.ProductStatus = true;
                product.CategoryID = 1;

                c.Add(product);
                c.SaveChanges();


                return RedirectToAction("Index", "Product");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                }
            }

            return View();
        }


    }
}
