using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;

namespace Todo_TailWindBlazor.Web.Pages
{
    public partial class Todo : ComponentBase
    {
        private string newTask = string.Empty;
        private List<TodoTask> Tasks = new();

        protected override void OnInitialized()
        {
            // Carregar tarefas do armazenamento local (poderia ser de um banco futuramente)
            Tasks = TasksStorage.Load();
            RemoveExpiredCompletedTasks();
        }

        private void AddTask()
        {
            if (!string.IsNullOrWhiteSpace(newTask))
            {
                Tasks.Add(new TodoTask
                {
                    Id = Guid.NewGuid(),
                    Text = newTask.Trim(),
                    CreatedAt = DateTime.Now,
                    Completed = false
                });
                newTask = string.Empty;
                SaveTasks();
            }
        }

        private void CompleteTask(TodoTask task)
        {
            task.Completed = true;
            task.CompletedAt = DateTime.Now;
            task.Selected = false;
            SaveTasks();
        }

        private void CompleteSelected()
        {
            foreach (var task in PendingTasks.Where(t => t.Selected))
            {
                CompleteTask(task);
            }
        }

        private void ToggleSelectAllPending(ChangeEventArgs e)
        {
            bool selectAll = (bool)e.Value;
            foreach (var task in PendingTasks)
            {
                task.Selected = selectAll;
            }
        }

        private void OnKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                AddTask();
            }
        }

        private void SaveTasks()
        {
            TasksStorage.Save(Tasks);
            RemoveExpiredCompletedTasks();
            StateHasChanged();
        }

        private void RemoveExpiredCompletedTasks()
        {
            var now = DateTime.Now;
            Tasks.RemoveAll(t => t.Completed && t.CompletedAt.HasValue && (now - t.CompletedAt.Value).TotalDays > 7);
        }

        private int GetDaysRemaining(DateTime? completedAt)
        {
            if (completedAt == null) return 0;
            var expiry = completedAt.Value.AddDays(7);
            var days = (expiry - DateTime.Now).Days;
            return days > 0 ? days : 0;
        }

        private List<TodoTask> PendingTasks => Tasks.Where(t => !t.Completed).ToList();
        private List<TodoTask> CompletedTasks => Tasks.Where(t => t.Completed).ToList();
        private int SelectedPendingCount => PendingTasks.Count(t => t.Selected);
        private bool AllPendingSelected => PendingTasks.Count > 0 && PendingTasks.All(t => t.Selected);

        public class TodoTask
        {
            public Guid Id { get; set; }
            public string Text { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public bool Completed { get; set; }
            public DateTime? CompletedAt { get; set; }
            public bool Selected { get; set; }
        }

        // Simulação de armazenamento local (pode ser substituído por banco de dados futuramente)
        public static class TasksStorage
        {
            private static List<TodoTask>? _cache;
            public static List<TodoTask> Load()
            {
                if (_cache != null) return _cache;
                // Aqui poderia ser implementado o carregamento de um banco ou arquivo
                _cache = new List<TodoTask>();
                return _cache;
            }
            public static void Save(List<TodoTask> tasks)
            {
                _cache = tasks;
            }
        }
    }
}
