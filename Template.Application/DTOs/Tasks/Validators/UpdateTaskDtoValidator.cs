using FluentValidation;
using Template.Application.DTOs.Tasks;
using Template.Application.DTOs.Tasks.Validators;

namespace Template.Application.DTOs.Tasks.Validators
{
    public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskDtoValidator()
        {
            Include(new ITaskDtoValidator());

            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull().WithMessage("{PropertyName} must be present.");
        }
    }
}
