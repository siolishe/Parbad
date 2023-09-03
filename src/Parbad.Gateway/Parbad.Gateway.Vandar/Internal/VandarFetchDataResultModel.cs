namespace Parbad.Gateway.Vandar.Internal;

public class VandarFetchDataResultModel
{
    public bool Status { get; set; }
    public decimal Amount { get; set; }
    public long TransId { get; set; }
    public string Refnumber { get; set; }
    public string TrackingCode { get; set; }
    public string FactorNumber { get; set; }
    public string Mobile { get; set; }
    public string Description { get; set; }
    public string CardNumber { get; set; }
    public string CID { get; set; }
    public string CreatedAt { get; set; }
    public string PaymentDate { get; set; }
    public int Code { get; set; }
    public string Message { get; set; }
    public bool IsSucceed { get; set; }
}