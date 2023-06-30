using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BlazorInventory.Data
{
    public class ItemRevised
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int SalesCount { get; set; }
        public string ResponsibleIdentifier { get; set; }
        public int SubGroupId { get; set; }
        public DateTime? RevisionDate { get; set; }
    }
}
