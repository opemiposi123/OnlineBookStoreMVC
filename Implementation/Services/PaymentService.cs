using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Models.ResponseModels;

public class PaymentService
{
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _context;

    public PaymentService(HttpClient httpClient, ApplicationDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }
    public async Task<PaymentResponse> InitializePaymentAsync(decimal amount, string email, string callbackUrl, string orderId)
    {
        var request = new
        {
            email = email,
            amount = (int)(amount * 100), // Amount in kobo (Nigerian currency subunit)
            callback_url = callbackUrl,
            reference = Guid.NewGuid().ToString(), // Generate a unique reference
            orderId = orderId
        };

        var response = await _httpClient.PostAsJsonAsync("https://api.paystack.co/transaction/initialize", request);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Log the full response for debugging
        Console.WriteLine($"Raw Paystack Response: {responseContent}");

        var jsonDocument = JsonDocument.Parse(responseContent);
        string authorizationUrl = jsonDocument.RootElement.GetProperty("data").GetProperty("authorization_url").GetString();
        string reference = jsonDocument.RootElement.GetProperty("data").GetProperty("reference").GetString();

        Console.WriteLine($"Extracted Authorization URL: {authorizationUrl}");
        Console.WriteLine($"Extracted Reference: {reference}");

        var paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Validate the deserialized PaymentResponse object
        if (paymentResponse == null)
        {
            throw new InvalidOperationException("Failed to deserialize Paystack response.");
        }
        if (paymentResponse.Data == null)
        {
            throw new InvalidOperationException("The Data object in the Paystack response is null.");
        }
        if (string.IsNullOrEmpty(paymentResponse.Data.AuthorizationUrl))
        {
            throw new InvalidOperationException($"The AuthorizationUrl is missing or invalid. Error: {paymentResponse.Message}");
        }

        // Save payment details to the database
        var payment = new Payment
        {
            UserId = email,
            Amount = amount,
            OrderId = orderId,
            Reference = reference,  // Use the reference returned from Paystack
            Status = "Successful"
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return paymentResponse;
    }

    public async Task<VerifyTransactionResponse> VerifyPaymentAsync(string reference)
    {
        try
        {
            // Make the GET request to verify the transaction
            var response = await _httpClient.GetAsync($"https://api.paystack.co/transaction/verify/{reference}");
            response.EnsureSuccessStatusCode(); // Ensure the response is successful

            var responseContent = await response.Content.ReadAsStringAsync();

            // Deserialize the response into VerifyTransactionResponse object
            var verifyResponse = JsonSerializer.Deserialize<VerifyTransactionResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Check if the verification response is successful and the transaction status is 'success'
            if (verifyResponse != null && verifyResponse.Status && verifyResponse.Data.Status == "success")
            {
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Reference == reference);
                if (payment != null)
                {
                    payment.Status = "Completed"; // Update payment status
                    await _context.SaveChangesAsync(); // Save changes to the database
                }
            }

            return verifyResponse; // Return the verification response
        }
        catch (HttpRequestException httpEx)
        {
            // Log and handle HTTP request-specific exceptions
            Console.WriteLine($"HTTP Request Error: {httpEx.Message}");
            throw; // Optionally rethrow the exception to handle it further up the call stack
        }
        catch (JsonException jsonEx)
        {
            // Log and handle JSON deserialization errors
            Console.WriteLine($"JSON Deserialization Error: {jsonEx.Message}");
            throw; // Optionally rethrow the exception to handle it further up the call stack
        }
        catch (Exception ex)
        {
            // Log and handle any other unexpected errors
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            throw; // Optionally rethrow the exception to handle it further up the call stack
        }
    }

}
