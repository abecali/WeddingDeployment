using System.ComponentModel.DataAnnotations;

namespace wedding_planner.Models
{
    public class Association
    {
        [Key]
        public int AssociationId { get; set; }
        public int UserId { get; set; }
        public int WeddingId { get; set; }
        public User Guest { get; set; }  // why did we put this here?
        public Wedding Wedding { get; set; }// why did we put this here?

    }
}