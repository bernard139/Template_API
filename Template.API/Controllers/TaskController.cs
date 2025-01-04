using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Application.DTOs.Tasks;
using Template.Application.Responses;
using static Template.Application.Features.Tasks.CreateTask;
using static Template.Application.Features.Tasks.DeleteTask;
using static Template.Application.Features.Tasks.GetTask;
using static Template.Application.Features.Tasks.GetTaskList;
using static Template.Application.Features.Tasks.UpdateTask;

namespace Template.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator, User")]

    public class TaskController : BaseController
    {
        private readonly IMediator _mediator;

        public TaskController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskModel>>> Get()
        {
            GetTaskListQuery query = new GetTaskListQuery { UserId = GetCurrentUserId() };
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskModel>> Get(long id)
        {
            GetTaskQuery query = new GetTaskQuery { Id = id };
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServerResponse<bool>>> Create([FromBody] CreateTaskDto createTaskDto)
        {
            CreateTaskCommand command = new CreateTaskCommand
            {
                UserId = GetCurrentUserId(),
                CreateTaskDto = createTaskDto
            };
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<ServerResponse<bool>>> Update([FromBody] UpdateTaskDto updateTaskDto)
        {
            UpdateTaskCommand command = new UpdateTaskCommand { UpdateTaskDto = updateTaskDto };
            var response = await _mediator.Send(command);

            return Ok(response);
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServerResponse<bool>>> Delete(long id)
        {
            DeleteTaskCommand command = new DeleteTaskCommand { Id = id };
            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
