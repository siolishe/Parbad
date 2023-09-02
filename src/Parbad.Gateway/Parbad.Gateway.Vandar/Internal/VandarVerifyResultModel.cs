namespace Parbad.Gateway.Vandar.Internal;

public class VandarVerifyResultModel
{
    public bool Status { get; set; }
    public decimal Amount { get; set; }
    public int RealAmount { get; set; }
    public int Wage { get; set; }
    public long TransId { get; set; }
    public string FactorNumber { get; set; }
    public string Mobile { get; set; }
    public string Description { get; set; }
    public string CardNumber { get; set; }
    public string PaymentDate { get; set; }
    public string Cid { get; set; }
    public string Message { get; set; }
}