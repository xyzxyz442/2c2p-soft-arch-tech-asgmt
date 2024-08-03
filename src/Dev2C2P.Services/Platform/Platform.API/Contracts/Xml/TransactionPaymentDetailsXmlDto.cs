public record TransactionPaymentDetailsXmlDto
{
    [XmlElement("Amount")]
    public decimal Amount { get; init; } = 0;

    [XmlElement("CurrencyCode")]
    public string CurrencyCode { get; init; } = string.Empty;
}
