using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.Contracts.Persistence;
using Template.Application.Exceptions;
using Template.Application.Responses;
using static Template.Application.Features.Tasks.CreateTask;

namespace Template.Application.Features.Tasks
{
    public class DeleteTask
    {
        public class DeleteTaskCommand : IRequest<ServerResponse<bool>>
        {
            public long Id { get; set; }
        }

        public class DeleteTaskCommandHandler : ResponseBaseService, IRequestHandler<DeleteTaskCommand, ServerResponse<bool>>
        {
            private readonly IUnitOfWork _unitOfWork;
            public DeleteTaskCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public async Task<ServerResponse<bool>> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
            {
                var response = new ServerResponse<bool>();

                Domain.Task task = await _unitOfWork.TaskRepository.GetAsync(request.Id);
                if (task == null)
                {
                    return SetError(response, responseDescs.NULL_REFERENCE);
                };

                task.IsDeleted = true;
                await _unitOfWork.TaskRepository.UpdateAsync(task);

                return SetSuccess(response, true, responseDescs.SUCCESS);
            }
        }
    }
}
