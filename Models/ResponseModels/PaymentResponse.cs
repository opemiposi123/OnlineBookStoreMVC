using System.Text.Json.Serialization;

public class PaymentResponse
{
    public bool Status { get; set; }
    public string Message { get; set; }
    public PaymentData Data { get; set; }
}

public class PaymentData
{
    [JsonPropertyName("authorization_url")]
    public string AuthorizationUrl { get; set; }

    public string Reference { get; set; }
    public string AccessCode { get; set; }
}
