using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.Contracts.Identity;
using Template.Application.Contracts.Persistence;
using Template.Application.DTOs.Tasks;
using Template.Application.Responses;

namespace Template.Application.Features.Tasks
{
    public class GetTaskList
    {
        public class GetTaskListQuery : IRequest<ServerResponse<List<TaskModel>>>
        {
            public string UserId { get; set; } = string.Empty;
        }

        public class GetTaskListQueryHandler : ResponseBaseService, IRequestHandler<GetTaskListQuery, ServerResponse<List<TaskModel>>>
        {
            private readonly ITaskRepository _taskRepository;

            public GetTaskListQueryHandler(ITaskRepository taskRepository)
            {
                _taskRepository = taskRepository;
            }

            public async Task<ServerResponse<List<TaskModel>>> Handle(GetTaskListQuery request, CancellationToken cancellationToken)
            {
                var response = new ServerResponse<List<TaskModel>>();

                IReadOnlyList<Domain.Task> tasks = await _taskRepository
                    .GetAllAsync()
                    .ConfigureAwait(false);

                List<TaskModel> taskModels = tasks
                    .Where(x => x.CreatedBy == request.UserId)
                    .Select(x => x.Adapt<TaskModel>())
                    .ToList();

                return SetSuccess(response, taskModels, responseDescs.SUCCESS);
            }

        }
    }
}
