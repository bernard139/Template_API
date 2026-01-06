using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Template.Application.Contracts.Persistence;
using Template.Application.DTOs.Tasks;
using Template.Application.Responses;
using Mapster;

namespace Template.Application.Features.Tasks
{
    public class GetTaskList
    {
        public class GetTaskListQuery : IRequest<ServerResponse<List<TaskModel>>>
        {
            public string UserId { get; set; } = string.Empty;
            public TaskDto TaskDto { get; set; } = new TaskDto();
        }

        public class GetTaskListQueryHandler : ResponseBaseService, IRequestHandler<GetTaskListQuery, ServerResponse<List<TaskModel>>>
        {
            private readonly ITaskRepository _taskRepository;

            public GetTaskListQueryHandler(ITaskRepository taskRepository)
            {
                _taskRepository = taskRepository;
            }

            public async Task<ServerResponse<List<TaskModel>>> Handle(
                GetTaskListQuery request,
                CancellationToken cancellationToken)
            {
                var pageNumber = Math.Max(request.TaskDto.PageNumber, 1);
                var pageSize = Math.Min(Math.Max(request.TaskDto.PageSize, 1), 100);

                var allTasks = (await _taskRepository.GetAllAsync().ConfigureAwait(false))
                    .Where(t => t.IsDeleted == false)
                    .ToList();

                var filteredTasks = allTasks
                    .Where(t => t.CreatedBy == request.UserId)
                    .AsEnumerable();

                if (!string.IsNullOrWhiteSpace(request.TaskDto.Name))
                {
                    var nameFilter = request.TaskDto.Name.Trim().ToLower();
                    filteredTasks = filteredTasks.Where(t =>
                        t.Name != null && t.Name.ToLower().Contains(nameFilter));
                }

                var totalCount = filteredTasks.Count();

                var taskModels = filteredTasks
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => x.Adapt<TaskModel>())
                    .ToList();

                var response = new ServerResponse<List<TaskModel>>();
                return SetSuccess(response, taskModels, responseDescs.SUCCESS);
            }
        }
    }
}