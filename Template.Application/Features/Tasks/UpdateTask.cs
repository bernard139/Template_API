using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.Contracts.Persistence;
using Template.Application.DTOs.Tasks.Validators;
using Template.Application.DTOs.Tasks;
using Template.Application.Exceptions;
using Template.Application.Responses;
using Mapster;

namespace Template.Application.Features.Tasks
{
    public class UpdateTask
    {
        public class UpdateTaskCommand : IRequest<ServerResponse<bool>>
        {
            public UpdateTaskDto UpdateTaskDto { get; set; } = null!;
        }

        public class UpdateTaskCommandHandler : ResponseBaseService, IRequestHandler<UpdateTaskCommand, ServerResponse<bool>>
        {
            private readonly IUnitOfWork _unitOfWork;
            public UpdateTaskCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public async Task<ServerResponse<bool>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
            {
                var response = new ServerResponse<bool>();

                ITaskDtoValidator validator = new();
                var validationResult = await validator.ValidateAsync(request.UpdateTaskDto, cancellationToken);

                if (validationResult.IsValid == false)
                { return SetError(response, responseDescs.FAIL); }

                Domain.Task task = await _unitOfWork.TaskRepository.GetAsync(request.UpdateTaskDto.Id);
                if (task == null)
                { return SetError(response, responseDescs.NULL_REFERENCE); }

                var updateTask = request.UpdateTaskDto.Adapt<Domain.Task>();

                await _unitOfWork.TaskRepository.UpdateAsync(updateTask);
                await _unitOfWork.Save();

                return SetSuccess(response, true, responseDescs.SUCCESS);
            }
        }
    }
}
