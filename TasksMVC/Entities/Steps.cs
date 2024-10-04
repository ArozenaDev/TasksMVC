namespace TasksMVC.Entities
{
    public class Steps
    {
        public Guid Id { get; set; }
        public int TaskId { get; set; }
        public Tarea Task { get; set; }
        public string Description { get; set; }
        public bool Done { get; set; }
        public int Order { get; set; }  
    }
}
