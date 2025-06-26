using Microsoft.JSInterop;
using System.Text.Json;
using Todo_TailWindBlazor.Web.Dominio;

namespace Todo_TailWindBlazor.Web.Bd_Servico
{
    public class TasksStorage
    {
        private readonly IJSRuntime _jsRuntime;
        private const string StorageKey = "TodoTasks";

        public TasksStorage(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<List<TodoTask>> LoadTasksAsync()
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageKey);
            if (string.IsNullOrEmpty(json))
            {
                return new List<TodoTask>();
            }
            return JsonSerializer.Deserialize<List<TodoTask>>(json) ?? new List<TodoTask>();
        }

        public async Task SaveTasksAsync(List<TodoTask> tasks)
        {
            var json = JsonSerializer.Serialize(tasks);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
        }
    }
}
