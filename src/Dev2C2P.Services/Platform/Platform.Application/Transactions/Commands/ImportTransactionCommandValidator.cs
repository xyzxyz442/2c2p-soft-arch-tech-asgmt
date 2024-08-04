using System.Globalization;
using Dev2C2P.Services.Platform.Application.Transactions.Commands;
using Dev2C2P.Services.Platform.Common;
using FluentValidation;

public class ImportTransactionCommandValidator
    : AbstractValidator<ImportTransactionCommand>
{
    public ImportTransactionCommandValidator()
    {
        RuleFor(x => x.Type).IsInEnum().Must(x => x != ImportTransactionFileType.None);
        RuleFor(x => x.Datas).NotEmpty();

        When(x => x.Type == ImportTransactionFileType.Csv, () =>
        {
            RuleForEach(x => x.Datas).ChildRules(
                validator =>
                {
                    validator.RuleFor(x => x.Id)
                        .NotEmpty()
                        .MaximumLength(50);
                    validator.RuleFor(x => x.At)
                        .NotEmpty()
                        .Must(x => ValidateDateTime(x, "dd/MM/yyyy HH:mm:ss"));
                    validator.RuleFor(x => x.CurrencyCode)
                        .Must(x => ISO._4217.CurrencyCodesResolver.Codes
                            .Any(y => y.Code == x.ToUpperInvariant()));
                    validator.RuleFor(x => x.Status)
                        .NotEmpty()
                        .Must(x => new[] { "Approved", "Failed", "Finished" }.Contains(x));
                }
            );
        });

        When(x => x.Type == ImportTransactionFileType.Xml, () =>
        {
            RuleForEach(x => x.Datas).ChildRules(
                validator =>
                {
                    validator.RuleFor(x => x.Id)
                        .NotEmpty()
                        .MaximumLength(50);
                    validator.RuleFor(x => x.At)
                        .NotEmpty()
                        .Must(x => ValidateDateTime(x, "yyyy-MM-ddTHH:mm:ss"));
                    validator.RuleFor(x => x.CurrencyCode)
                        .Must(x => ISO._4217.CurrencyCodesResolver.Codes
                            .Any(y => y.Code == x.ToUpperInvariant()));
                    validator.RuleFor(x => x.Status).NotEmpty().Must(x => new[] { "Approved", "Rejected", "Done" }.Contains(x));
                }
            );
        });
    }

    private bool ValidateDateTime(string value, string format)
    {
        return DateTime.TryParseExact(
            value,
            format,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
    }
}
