namespace Web.Controllers.Todo
{
    using System.Threading.Tasks;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    using Swashbuckle.AspNetCore.Annotations;

    using Models.Todo;

    using Shared;

    using Web.Extentions;

    using Application.Handlers.TodoLists.Commands.UpdateTodoItem;
    using Application.Handlers.TodoLists.Commands.DeleteTodoItem;
    using Application.Handlers.TodoLists.Commands.CreateTodoItem;
    using Application.Handlers.TodoItems.Queries.GetTodos;

    [Authorize]
    public class TodoItemController : ApiController
    {
        [HttpGet(nameof(GetPagedTodosForUser))]
        [SwaggerOperation("Get all Paged Todos for Specific User.", "")]
        public async Task<ActionResult<PaginatedResult<TodoItemDto>>> GetPagedTodosForUser(
            int pageNumber,
            int pageSize,
            [Required] string userId,
            [Required] string listId)
        {
            return await Mediator.Send(new GetPagedTodosForUserQuery(pageNumber, pageSize, userId, listId));
        }

        [HttpGet(nameof(GetById))]
        [SwaggerOperation("Get Item By Id.", "")]
        public async Task<ActionResult<GetUserTodoItemDto>> GetById(
            [Required] string userId,
            [Required] string listId,
            [Required] string itemId)
        {
            return await Mediator.Send(new GetTodoItemByIdQuery(userId, listId, itemId)).ToActionResult();
        }

        [HttpPost(nameof(CreateTodo))]
        [SwaggerOperation("Create Todo Item.", "")]
        public async Task<ActionResult<TodoItemDto>> CreateTodo(CreateTodoItemCommand request)
        {
            return await Mediator.Send(request).ToActionResult();
        }

        [HttpPut(nameof(ToggleStatus))]
        [SwaggerOperation("Toggle status of Todo Item.", "")]
        public async Task<ActionResult<TodoItemDto>> ToggleStatus(ToggleTodoStatusCommand request)
        {
            return await Mediator.Send(request).ToActionResult();
        }

        [HttpPut(nameof(ChangePriority))]
        [SwaggerOperation("Change Todo Priority.", "")]
        public async Task<ActionResult<TodoItemDto>> ChangePriority(ChangePriorityTodoItemCommand request)
        {
            return await Mediator.Send(request).ToActionResult();
        }

        [HttpPut(nameof(ChangeOrder))]
        [SwaggerOperation("Change Todo Order.", "")]
        public async Task<ActionResult<TodoItemDto>> ChangeOrder(UpdateTodoItemOrderIndexCommand request)
        {
            return await Mediator.Send(request).ToActionResult();
        }

        [HttpPut(nameof(Reorder))]
        [SwaggerOperation("Change Todos Order.", "")]
        public async Task<ActionResult<TodoListDto>> Reorder(UpdateItemsOrderIndexCommand request)
        {
            return await Mediator.Send(request).ToActionResult();
        }

        [HttpPut(nameof(UpdateItem))]
        [SwaggerOperation("Update Todo Title or Note.", "")]
        public async Task<ActionResult<TodoItemDto>> UpdateItem(UpdateTodoDescrioptionCommand request)
        {
            return await Mediator.Send(request).ToActionResult();
        }

        [HttpDelete(nameof(Delete))]
        [SwaggerOperation("Delete Todo Item.", "")]
        public async Task<ActionResult<string>> Delete([Required] string userId, [Required] string listId, [Required] string itemId)
        {
            return await Mediator.Send(new DeleteTodoItemCommand(userId, listId, itemId)).ToActionResult();
        }
    }
}