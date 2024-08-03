namespace Dev2C2P.Services.Platform.Contracts.Xml;

[XmlRoot("Transactions")]
public record TransactionsXmlDto
{
    [XmlElement("Transaction")]
    public List<TransactionXmlDto> Transactions { get; init; } = new List<TransactionXmlDto>();
}
