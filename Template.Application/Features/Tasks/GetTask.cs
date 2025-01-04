using Mapster;
using MediatR;
using Template.Application.Contracts.Persistence;
using Template.Application.DTOs.Tasks;
using Template.Application.Responses;

namespace Template.Application.Features.Tasks
{
    public class GetTask
    {
        public class GetTaskQuery : IRequest<ServerResponse<TaskModel>>
        {
            public long Id { get; set; }
        }

        public class GetTaskQueryHandler : ResponseBaseService, IRequestHandler<GetTaskQuery, ServerResponse<TaskModel>>
        {
            private readonly ITaskRepository _taskRepository;
            public GetTaskQueryHandler(ITaskRepository taskRepository)
            {
                _taskRepository = taskRepository;
            }

            public async Task<ServerResponse<TaskModel>> Handle(GetTaskQuery request, CancellationToken cancellationToken)
            {
                var response = new ServerResponse<TaskModel>();
                Domain.Task task = await _taskRepository
                    .GetAsync(request.Id)
                    .ConfigureAwait(false);

                if (task == null)
                {
                    return SetError(response, responseDescs.NULL_REFERENCE);
                }

                var data = task.Adapt<TaskModel>();

                return SetSuccess(response, data, responseDescs.SUCCESS);
            }
        }
    }
}
