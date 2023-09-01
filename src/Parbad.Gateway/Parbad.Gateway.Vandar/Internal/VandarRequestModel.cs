namespace Parbad.Gateway.Vandar.Internal;

public class VandarRequestModel
{
    public string api_key { get; set; }
    public int amount { get; set; }
    public string callback_url { get; set; }
    public string Mobile_Number { get; set; }
    public string FactorNumber { get; set; }
    public string Description { get; set; }
    public string National_Code { get; set; }
    public List<string> Valid_Card_Number { get; set; }
    public string Port { get; set; }
}