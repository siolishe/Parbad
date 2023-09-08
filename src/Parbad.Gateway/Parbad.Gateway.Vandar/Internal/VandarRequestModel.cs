namespace Parbad.Gateway.Vandar.Internal;

public class VandarRequestModel
{
    public string api_key { get; set; }
    public int amount { get; set; }
    public string callback_url { get; set; }
    public string mobile_number { get; set; }
    public string factorNumber { get; set; }
    public string description { get; set; }
    public string national_code { get; set; }
    public List<string> valid_card_number { get; set; }
    public string port { get; set; }
}