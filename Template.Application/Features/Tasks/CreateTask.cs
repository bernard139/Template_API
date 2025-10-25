using MediatR;
using Microsoft.AspNetCore.Http;
using Template.Application.Contracts.Infrastructure;
using Template.Application.Contracts.Persistence;
using Template.Application.DTOs.Tasks.Validators;
using Template.Application.DTOs.Tasks;
using Template.Application.Models;
using Template.Application.Responses;
using FluentValidation.Results;
using Mapster;

namespace Template.Application.Features.Tasks
{
    public class CreateTask
    {
        public class CreateTaskCommand : IRequest<ServerResponse<bool>>
        {
            public string UserId { get; set; } = string.Empty;
            public CreateTaskDto CreateTaskDto { get; set; } = null!;
        }

        public class CreateTaskCommandHandler : ResponseBaseService, IRequestHandler<CreateTaskCommand, ServerResponse<bool>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public CreateTaskCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<ServerResponse<bool>> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
            {
                var response = new ServerResponse<bool>();

                ITaskDtoValidator validator = new();
                ValidationResult validationResult = await validator.ValidateAsync(command.CreateTaskDto);
                if (!validationResult.IsValid)
                {
                    return SetError(response, responseDescs.FAIL);
                }

                Domain.Task newTask = command.CreateTaskDto.Adapt<Domain.Task>();

                newTask.CreatedBy = command.UserId;

                await _unitOfWork.TaskRepository.AddAsync(newTask);
                await _unitOfWork.Save();

                return SetSuccess(response, true, responseDescs.SUCCESS);
            }
        }
    }
}
