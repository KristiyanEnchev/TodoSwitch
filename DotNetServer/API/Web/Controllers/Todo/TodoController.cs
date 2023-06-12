namespace Web.Controllers.Todo
{
    using Microsoft.AspNetCore.Mvc;

    using Swashbuckle.AspNetCore.Annotations;

    using Models.Todo;

    using Web.Extentions;

    using Application.Handlers.TodoLists.Queries.GetTodos;

    using Domain.Entities;

    using Shared;

    public class TodoController : ApiController
    {
        [HttpGet(nameof(GetAll))]
        [SwaggerOperation("Get all Todos.", "")]
        public async Task<ActionResult<TodosVm>> GetAll()
        {
            return await Mediator.Send(new GetTodosQuery()).ToActionResult();
        }

        [HttpPost(nameof(GetPagedTodos))]
        [SwaggerOperation("Get all Paged Todos.", "")]
        public async Task<ActionResult<PaginatedResult<TodoList>>> GetPagedTodos(GetPagedTodosQuery request)
        {
            return await Mediator.Send(request);
        }

        [HttpPost(nameof(GetPagedTodosForUSer))]
        [SwaggerOperation("Get all Paged Todos for Specific User.", "")]
        public async Task<ActionResult<PaginatedResult<TodoListDto>>> GetPagedTodosForUSer(GetPagedTodosForUSerQuery request)
        {
            return await Mediator.Send(request);
        }
    }
}