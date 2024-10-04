using Microsoft.EntityFrameworkCore;

namespace TasksMVC.Entities
{
    public class AttachedFile
    {
        public Guid Id { get; set; }
        public int TaskId { get; set; }
        public Tarea Task { get; set; }
        [Unicode]
        public string Url { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public DateTime Date { get; set; }
    }
}
