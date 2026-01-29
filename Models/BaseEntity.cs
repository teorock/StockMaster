using System.ComponentModel.DataAnnotations;

namespace StockMaster.Models
{
    public abstract class BaseEntity
    {
        [Display(Name = "Creato il")]
        public DateTime CreatedOn { get; set; }

        [StringLength(100)]
        [Display(Name = "Creato da")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Modificato il")]
        public DateTime? UpdatedOn { get; set; }

        [StringLength(100)]
        [Display(Name = "Modificato da")]
        public string? UpdatedBy { get; set; }
    }
}