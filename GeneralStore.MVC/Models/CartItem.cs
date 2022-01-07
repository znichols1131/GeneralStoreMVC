using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GeneralStore.MVC.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemID { get; set; }

        [Required, ForeignKey(nameof(Customer))]
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Required, ForeignKey(nameof(Product))]
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        [Display(Name = "Item Count")]
        public int ProductCount { get; set; }

        // Not required, we'll add this in the controller
        [Column(TypeName = "datetime2")]
        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }
    }
}