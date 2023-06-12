namespace Web.Controllers.Todo
{
    using System.Threading.Tasks;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Mvc;

    using Swashbuckle.AspNetCore.Annotations;

    using Models.Todo;

    using Application.Handlers.TodoLists.Queries.GetList;
    using Application.Handlers.TodoLists.Commands.CreateList;
    using Application.Handlers.TodoLists.Commands.DeleteList;
    using Application.Handlers.TodoLists.Commands.UpdateTodoList;

    using Web.Extentions;

    public class TodoListController : ApiController
    {
        [HttpGet(nameof(GetColors))]
        [SwaggerOperation("Get all Todo List Colors.", "")]
        public async Task<ActionResult<List<SupportedColorDto>>> GetColors()
        {
            return await Mediator.Send(new GetSupportedColorsQuery());
        }

        [HttpGet(nameof(GetForUser))]
        [SwaggerOperation("Get all Todo List.", "")]
        public async Task<ActionResult<List<GetUserTodoListsDto>>> GetForUser([Required] string userId)
        {
            return await Mediator.Send(new GetUserTodoListQuery(userId)).ToActionResult();
        }

        [HttpPost(nameof(Create))]
        [SwaggerOperation("Create Todo List.", "")]
        public async Task<ActionResult<TodoListDto>> Create(CreateListCommand request)
        {
            return await Mediator.Send(request).ToActionResult();
        }

        [HttpPut(nameof(UpdateTitle))]
        [SwaggerOperation("Update Todo List Title.", "")]
        public async Task<ActionResult<TodoListDto>> UpdateTitle(UpdateListTitleCommand request)
        {
            return await Mediator.Send(request).ToActionResult();
        }

        [HttpPut(nameof(UpdateColor))]
        [SwaggerOperation("Update Todo List Color.", "")]
        public async Task<ActionResult<TodoListDto>> UpdateColor(UpdateListColorCommand request)
        {
            return await Mediator.Send(request).ToActionResult();
        }

        [HttpDelete(nameof(Delete))]
        [SwaggerOperation("Delete Todo List.", "")]
        public async Task<ActionResult<string>> Delete([Required] string userId, [Required] string listId)
        {
            return await Mediator.Send(new DeleteListCommand(userId, listId)).ToActionResult();
        }
    }
}