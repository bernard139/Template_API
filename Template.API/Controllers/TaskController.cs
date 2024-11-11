//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Template.Application.DTOs.Tasks;
//using Template.Application.Features.Categories.Requests.Commands;
//using Template.Application.Features.Categories.Requests.Queries;
//using Template.Application.Responses;

//namespace Template.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize(Roles = "Administrator")]
//    public class TaskController : ControllerBase
//    {
//        private readonly IMediator _mediator;

//        public TaskController(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        [HttpGet]
//        public async Task<ActionResult<List<TaskDto>>> Get()
//        {
//            GetTaskListRequest query = new GetTaskListRequest();
//            List<TaskDto> response = await _mediator.Send(query);

//            return Ok(response);
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<TaskDto>> Get(int id)
//        {
//            GetTaskRequest query = new GetTaskRequest { Id = id };
//            TaskDto response = await _mediator.Send(query);

//            return Ok(response);
//        }

//        [HttpPost]
//        public async Task<ActionResult<BaseCommandResponse>> Post([FromBody] CreateTaskDto createTaskDto)
//        {
//            CreateTaskCommand command = new CreateTaskCommand { CreateTaskDto = createTaskDto };
//            BaseCommandResponse response = await _mediator.Send(command);

//            return Ok(response);
//        }

//        [HttpPut("{id}")]
//        public async Task<ActionResult> Put([FromBody] UpdateTaskDto updateTaskDto)
//        {
//            UpdateTaskCommand command = new UpdateTaskCommand {  UpdateTaskDto = updateTaskDto };
//            await _mediator.Send(command);

//            return NoContent();
//        }

//        [HttpDelete("{id}")]
//        public async Task<ActionResult> Delete(int id)
//        {
//            DeleteTaskCommand command = new DeleteTaskCommand { Id = id };
//            await _mediator.Send(command);

//            return NoContent();
//        }
//    }
//}
