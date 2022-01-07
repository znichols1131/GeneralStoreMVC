using GeneralStore.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeneralStore.MVC.Controllers
{
    public class TransactionController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Transaction
        public ActionResult Index()
        {
            List<Transaction> transactionList = _db.Transactions.ToList();
            List<Transaction> orderedList = transactionList.OrderByDescending(t => t.DateOfRecentActivity).ToList();
            return View(orderedList);
        }

        // GET: Transaction
        public ActionResult Create()
        {
            ViewBag.Customers = new SelectList(_db.Customers.OrderBy(c => c.FirstName).ToList(), "CustomerID", "FullName");
            ViewBag.Products = new SelectList(_db.Products.OrderBy(p => p.Name).ToList(), "ProductID", "Name");

            return View();
        }

        // Post: Transaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                // Make sure the item count is valid
                if(transaction.VerifyProductInStock(_db))
                {
                    transaction.DateCreated = DateTime.Now;
                    _db.Transactions.Add(transaction);

                    transaction.ConsumeInventory(_db);

                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Customers = new SelectList(_db.Customers.OrderBy(c => c.FirstName).ToList(), "CustomerID", "FullName");
            ViewBag.Products = new SelectList(_db.Products.OrderBy(p => p.Name).ToList(), "ProductID", "Name");

            return View(transaction);
        }

        // GET: Delete
        // Transaction/Delete/{id}
        public ActionResult Delete(int? id)
        {
            if (id is null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Transaction transaction = _db.Transactions.Find(id);
            if (transaction is null)
            {
                return HttpNotFound();
            }

            return View(transaction);
        }

        // POST: Transaction
        // Transaction/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Transaction transaction = _db.Transactions.Find(id);

            transaction.ReturnInventory(_db);

            _db.Transactions.Remove(transaction);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Edit
        // Transaction/Edit/{id}
        public ActionResult Edit(int? id)
        {
            if (id is null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Transaction transaction = _db.Transactions.Find(id);
            if (transaction is null)
            {
                return HttpNotFound();
            }

            ViewBag.Customers = new SelectList(_db.Customers.OrderBy(c => c.FirstName).ToList(), "CustomerID", "FullName");
            ViewBag.Products = new SelectList(_db.Products.OrderBy(p => p.Name).ToList(), "ProductID", "Name");

            return View(transaction);
        }

        // POST: Edit
        // Transaction/Edit/{id}
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var tempTransaction = _db.Transactions.Find(transaction.TransactionID);
                _db.Entry(tempTransaction).State = System.Data.Entity.EntityState.Detached;
                int productCountBefore = tempTransaction.ProductCount;

                if (transaction.VerifyProductInStock(_db, productCountBefore))
                {
                    transaction.DateUpdated = DateTime.Now;
                    _db.Entry(transaction).State = System.Data.Entity.EntityState.Modified;

                    transaction.UpdateInventory(_db, productCountBefore);

                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Customers = new SelectList(_db.Customers.OrderBy(c => c.FirstName).ToList(), "CustomerID", "FullName");
            ViewBag.Products = new SelectList(_db.Products.OrderBy(p => p.Name).ToList(), "ProductID", "Name");

            return View(transaction);
        }

        // GET: Details
        // Transaction/Details/{id}
        public ActionResult Details(int? id)
        {
            if (id is null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Transaction transaction = _db.Transactions.Find(id);
            if (transaction is null)
            {
                return HttpNotFound();
            }

            return View(transaction);
        }
    }
}