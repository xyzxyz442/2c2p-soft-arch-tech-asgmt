namespace Dev2C2P.Services.Platform.Contracts.Transactions.Dtos.Xml;

public record TransactionPaymentDetailsXmlDto
{
    [XmlElement("Amount")]
    public decimal Amount { get; init; } = 0;

    [XmlElement("CurrencyCode")]
    public string CurrencyCode { get; init; } = string.Empty;
}
