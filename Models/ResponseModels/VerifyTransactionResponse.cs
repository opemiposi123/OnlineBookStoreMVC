namespace OnlineBookStoreMVC.Models.ResponseModels
{
    public class VerifyTransactionResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public VerifyTransactionData Data { get; set; }
    }

    public class VerifyTransactionData
    {
        public string Status { get; set; }
        public string Reference { get; set; }
        public string Amount { get; set; }
        public string GatewayResponse { get; set; }
    }
}
