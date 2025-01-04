using FluentValidation;

namespace Template.Application.DTOs.Tasks.Validators
{
    public class ITaskDtoValidator : AbstractValidator<ITaskDto>
    {
        public ITaskDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull();

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull();

            //RuleFor(x => x.PriorityId)
            //    .NotEmpty().WithMessage("{PropertyName} is required.")
            //    .NotNull()
            //    .MustAsync(async (id, token) =>
            //    {
            //        var taskExists = await _taskRepository.Exists(id);
            //        return !taskExists;
            //    });
        }
    }
}
