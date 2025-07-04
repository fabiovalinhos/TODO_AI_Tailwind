using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Todo_TailWindBlazor.Web.Dominio;
using Todo_TailWindBlazor.Web.Bd_Servico;

namespace Todo_TailWindBlazor.Web.Pages
{
    public partial class Todo : ComponentBase
    {
        [Inject]
        public TasksStorage? TasksStorage { get; set; }

        private string _newTask = string.Empty;
        public string newTask
        {
            get => _newTask;
            set => _newTask = value;
        }

        private List<TodoTask> Tasks = new();

        protected override async Task OnInitializedAsync()
        {

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (TasksStorage != null)
                {
                    Tasks = await TasksStorage.LoadTasksAsync();
                    await RemoveExpiredCompletedTasksAsync();
                }
                // Remover await SaveTasksAsync(); aqui para não sobrescrever input
            }
        }

        private async Task AddTaskAsync()
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
                await SaveTasksAsync();
            }
        }

        private async Task CompleteTaskAsync(TodoTask task)
        {
            task.Completed = true;
            task.CompletedAt = DateTime.Now;
            task.Selected = false;
            await SaveTasksAsync();
        }

        private async Task CompleteSelectedAsync()
        {
            foreach (var task in PendingTasks.Where(t => t.Selected))
            {
                await CompleteTaskAsync(task);
            }
        }

        private async Task OnKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await AddTaskAsync();
            }
        }

        private async Task SaveTasksAsync()
        {
            if (TasksStorage != null)
            {
                await TasksStorage.SaveTasksAsync(Tasks);
                await RemoveExpiredCompletedTasksAsync();
                StateHasChanged();
            }
        }

        private async Task RemoveExpiredCompletedTasksAsync()
        {
            var now = DateTime.Now;
            Tasks.RemoveAll(t => t.Completed && t.CompletedAt.HasValue && (now - t.CompletedAt.Value).TotalDays > 7);
            if (TasksStorage != null)
            {
                await TasksStorage.SaveTasksAsync(Tasks); // Save changes after removing expired tasks
            }
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

        private void ToggleSelectAllPending(ChangeEventArgs e)
        {
            bool selectAll = !AllPendingSelected;
            foreach (var task in PendingTasks)
            {
                task.Selected = selectAll;
            }
        }
    }
}