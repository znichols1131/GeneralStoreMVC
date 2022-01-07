using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GeneralStore.MVC.Models
{
    public class Transaction /*: IValidatableObject*/
    {
        [Key]
        public int TransactionID { get; set; }

        // Not required, we'll add this in the controller
        [Column(TypeName = "datetime2")]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        // Not required, we'll add this in the controller
        [Column(TypeName = "datetime2")]
        [Display(Name = "Date Updated")]
        public DateTime? DateUpdated { get; set; }

        public DateTime DateOfRecentActivity { 
            get 
            {
                return (DateUpdated is null) ? DateCreated : (DateTime)DateUpdated;
            } 
        }

        [Required, ForeignKey(nameof(Customer))]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Required, ForeignKey(nameof(Product))]
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        [Display(Name = "Item Count")]
        public int ProductCount { get; set; }

        public bool VerifyProductInStock(ApplicationDbContext _db)
        {
            int inventoryCount = _db.Products.Find(ProductID).InventoryCount;
            return ProductCount <= inventoryCount;
        }

        public bool VerifyProductInStock(ApplicationDbContext _db, int productCountBefore)
        {
            int inventoryCount = _db.Products.Find(ProductID).InventoryCount;
            return ProductCount <= (inventoryCount + productCountBefore);
        }

        public void ConsumeInventory(ApplicationDbContext _db)
        {
            Product product = _db.Products.Find(ProductID);

            product.InventoryCount -= ProductCount;

            // Will save outside of this method
        }

        public void UpdateInventory(ApplicationDbContext _db, int productCountBefore)
        {
            Product product = _db.Products.Find(ProductID);

            product.InventoryCount += productCountBefore;
            product.InventoryCount -= ProductCount;

            // Will save outside of this method
        }

        public void ReturnInventory(ApplicationDbContext _db)
        {
            Product product = _db.Products.Find(ProductID);

            product.InventoryCount += ProductCount;

            // Will save outside of this method
        }
    }
}