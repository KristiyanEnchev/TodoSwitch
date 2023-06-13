namespace Application.Interfaces.Services
{
    using Domain.Entities;

    public interface IOrderService
    {
        void ReorderItems(List<TodoItem> items, string itemId, int newOrderIndex);
        void ReorderLists(List<TodoList> lists, string listId, int newOrderIndex);
    }
}