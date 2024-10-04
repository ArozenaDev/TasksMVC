using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TasksMVC.Entities
{
    public class Tarea
    {
        public int Id { get; set; }
        [StringLength(250)]
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public DateTime Date { get; set; }
        public string CreatorId { get; set; }
        public IdentityUser Creator { get; set; }
        public List<Steps> Steps { get; set; }
        public List<AttachedFile> AttachedFiles { get; set; }
    }
}
