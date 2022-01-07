using GeneralStore.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeneralStore.MVC.Controllers
{
    public class CartItemController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        // GET: CartItem
        public ActionResult Index()
        {
            List<CartItem> cartItems = _db.CartItems.ToList();
            List<CartItem> orderedList = cartItems.OrderByDescending(t => t.Product.Name).ToList();

            return View(orderedList);
        }

        // GET: CartItem
        //// CartItem/CreateForProduct/{id}
        public ActionResult CreateForProduct(int productID)
        {
            Product product = _db.Products.Find(productID);

            CartItem newItem = new CartItem();
            newItem.Product = product;
            newItem.ProductID = product.ProductId;
            newItem.DateAdded = DateTime.Now;

            ViewBag.Customers = new SelectList(_db.Customers.OrderBy(c => c.FirstName).ToList(), "CustomerID", "FullName");

            return View(newItem);
        }

        // Post: CartItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateForProduct(CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                _db.CartItems.Add(cartItem);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Customers = new SelectList(_db.Customers.OrderBy(c => c.FirstName).ToList(), "CustomerID", "FullName");
            cartItem.Product = _db.Products.Find(cartItem.ProductID);

            return View(cartItem);
        }

        // GET: Delete
        // CartItem/Delete/{id}
        public ActionResult Delete(int? id)
        {
            if (id is null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            CartItem cartItem = _db.CartItems.Find(id);
            if (cartItem is null)
            {
                return HttpNotFound();
            }

            return View(cartItem);
        }

        // POST: CartItem
        // CartItem/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            CartItem cartItem = _db.CartItems.Find(id);
            _db.CartItems.Remove(cartItem);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Edit
        // CartItem/Edit/{id}
        public ActionResult Edit(int? id)
        {
            if (id is null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            CartItem cartItem = _db.CartItems.Find(id);
            if (cartItem is null)
            {
                return HttpNotFound();
            }

            ViewBag.Customers = new SelectList(_db.Customers.OrderBy(c => c.FirstName).ToList(), "CustomerID", "FullName");
            cartItem.Product = _db.Products.Find(cartItem.ProductID);

            return View(cartItem);
        }

        // POST: Edit
        // CartItem/Edit/{id}
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                cartItem.DateAdded = DateTime.Now;
                _db.Entry(cartItem).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Customers = new SelectList(_db.Customers.OrderBy(c => c.FirstName).ToList(), "CustomerID", "FullName");
            cartItem.Product = _db.Products.Find(cartItem.ProductID);

            return View(cartItem);
        }

        public ActionResult CompletePurchase()
        {
            List<CartItem> cart = _db.CartItems.ToList();

            if(cart.Count == 0)
            {
                ModelState.AddModelError("EmptyError", "There are no items to purchase.");
                return RedirectToAction("Index");
            }

            while (cart.Any())
            {
                CartItem cartItem = cart.First();
                Transaction transaction = GetTransactionForCartItem(cartItem);
                
                if(transaction.VerifyProductInStock(_db))
                {
                    _db.CartItems.Remove(cartItem);
                    _db.Transactions.Add(transaction);
                    transaction.ConsumeInventory(_db);
                }else
                {
                    ModelState.AddModelError("CartEmpty", $"There is not enough inventory to fulfill this order for {transaction.Product.Name}.");
                }

                cart.Remove(cartItem);
            }

            _db.SaveChanges();

            TempData["ModelState"] = ModelState;
            return RedirectToAction("Index");
        }

        public Transaction GetTransactionForCartItem(CartItem cartItem)
        {
            Transaction transaction = new Transaction();

            transaction.Customer = cartItem.Customer;
            transaction.CustomerID = cartItem.CustomerID;
            transaction.Product = cartItem.Product;
            transaction.ProductID = cartItem.ProductID;
            transaction.ProductCount = cartItem.ProductCount;
            transaction.DateCreated = DateTime.Now;

            return transaction;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (TempData["ModelState"] != null && !ModelState.Equals(TempData["ModelState"]))
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);

            base.OnActionExecuted(filterContext);
        }
    }
}