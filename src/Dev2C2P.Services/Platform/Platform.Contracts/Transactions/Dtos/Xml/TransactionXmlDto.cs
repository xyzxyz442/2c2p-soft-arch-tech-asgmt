namespace Dev2C2P.Services.Platform.Contracts.Transactions.Dtos.Xml;

public record TransactionXmlDto
{
    [XmlAttribute("id")]
    public string TransactionId { get; init; } = string.Empty;

    [XmlElement("TransactionDate")]
    public string TransactionDate { get; init; } = string.Empty;

    [XmlElement("PaymentDetails")]
    public TransactionPaymentDetailsXmlDto PaymentDetails { get; init; } = new TransactionPaymentDetailsXmlDto();

    [XmlElement("Status")]
    public string Status { get; init; } = string.Empty;
}
