using System;

namespace Todo_TailWindBlazor.Web.Dominio
{
    public class TodoTask
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool Completed { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool Selected { get; set; }
    }
}
