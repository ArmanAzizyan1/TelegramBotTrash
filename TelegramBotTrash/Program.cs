//using Telegram.Bot.Types.ReplyMarkups;
//using Telegram.Bot.Types;
//using Telegram.Bot;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllers();

//var app = builder.Build();

//// Bot token and client setup
//var botClient = new TelegramBotClient("7798238582:AAFlDOPBn1qmrG8adDcUIn8OjoorTh7iBlI"); // Replace with your bot token

//// File paths
//string addressFile = "addresses.txt";
//string paymentLogsFile = "payments.txt";

//// Start command handler
//async Task StartCommandHandler(Update update)
//{
//    var username = update.Message?.Chat.Id.ToString(); // Use user's unique Telegram ID
//    bool addressRegistered = CheckAddressRegistration(username, addressFile);

//    if (!addressRegistered)
//    {
//        await botClient.SendTextMessageAsync(update.Message?.Chat.Id, "Welcome! Please provide your address to continue.");
//        // Prompt the user to enter their address
//        await botClient.SendTextMessageAsync(update.Message?.Chat.Id, "Please enter your address:");
//    }
//    else
//    {
//        // Address already registered, show options
//        var keyboard = new InlineKeyboardMarkup(new[]
//        {
//            new [] { InlineKeyboardButton.WithCallbackData("Change address", "change_address") },
//            new [] { InlineKeyboardButton.WithCallbackData("Pay", "pay") }
//        });
//        await botClient.SendTextMessageAsync(update.Message?.Chat.Id, "You already have an address registered. Choose an action:", replyMarkup: keyboard);
//    }
//}

//// Save address handler
//async Task SaveAddressHandler(Update update)
//{
//    var userId = update.Message?.Chat.Id.ToString();
//    var address = update.Message?.Text;

//    if (string.IsNullOrWhiteSpace(address)) return;

//    SaveAddress(userId, address, addressFile);

//    var keyboard = new InlineKeyboardMarkup(new[]
//    {
//        new [] { InlineKeyboardButton.WithCallbackData("Pay", "pay") },
//        new [] { InlineKeyboardButton.WithCallbackData("Change address", "change_address") }
//    });

//    await botClient.SendTextMessageAsync(update.Message?.Chat.Id, "Your address has been saved! Choose an option:", replyMarkup: keyboard);
//}

//// Change address handler
//async Task ChangeAddressHandler(Update update)
//{
//    await botClient.SendTextMessageAsync(update.CallbackQuery?.Message?.Chat.Id, "Please provide your new address:");
//}

//// Pay handler
//async Task PayHandler(Update update)
//{
//    var userId = update.CallbackQuery?.From.Id.ToString();
//    var address = GetUserAddress(userId, addressFile);

//    if (address == null)
//    {
//        await botClient.SendTextMessageAsync(update.CallbackQuery?.Message?.Chat.Id, "You need to provide an address first.");
//        return;
//    }

//    string paymentId = GeneratePaymentId(paymentLogsFile);
//    LogPayment(userId, address, paymentId, paymentLogsFile);

//    var keyboard = new InlineKeyboardMarkup(new[]
//    {
//        new [] { InlineKeyboardButton.WithCallbackData("Start", "start") }
//    });

//    await botClient.SendTextMessageAsync(update.CallbackQuery?.Message?.Chat.Id, $"Payment registered! Payment ID: {paymentId}", replyMarkup: keyboard);
//}

//// Helper functions to manage addresses and payments
//bool CheckAddressRegistration(string userId, string addressFile)
//{
//    if (File.Exists(addressFile))
//    {
//        var lines = File.ReadAllLines(addressFile);
//        foreach (var line in lines)
//        {
//            if (line.StartsWith($"{userId} -"))
//                return true;
//        }
//    }
//    return false;
//}

//void SaveAddress(string userId, string address, string addressFile)
//{
//    var lines = File.Exists(addressFile) ? File.ReadAllLines(addressFile).ToList() : new List<string>();

//    // Check if the user already has an address
//    var existingLine = lines.FirstOrDefault(line => line.StartsWith($"{userId} -"));
//    if (existingLine != null)
//    {
//        lines.Remove(existingLine); // Remove old address
//    }

//    lines.Add($"{userId} - {address}");
//    File.WriteAllLines(addressFile, lines);
//}

//string GetUserAddress(string userId, string addressFile)
//{
//    if (File.Exists(addressFile))
//    {
//        var lines = File.ReadAllLines(addressFile);
//        var line = lines.FirstOrDefault(l => l.StartsWith($"{userId} -"));
//        return line?.Split("-")[1].Trim();
//    }
//    return null;
//}

//string GeneratePaymentId(string paymentLogsFile)
//{
//    string lastPaymentId = "000000000";
//    if (File.Exists(paymentLogsFile))
//    {
//        var lines = File.ReadAllLines(paymentLogsFile);
//        if (lines.Length > 0)
//        {
//            var lastLine = lines.Last();
//            lastPaymentId = lastLine.Split("-").Last();
//        }
//    }
//    int newPaymentIdNumber = int.Parse(lastPaymentId) + 1;
//    return DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "-" + newPaymentIdNumber.ToString("D9");
//}

//void LogPayment(string userId, string address, string paymentId, string paymentLogsFile)
//{
//    File.AppendAllText(paymentLogsFile, $"{userId}-{address}-{paymentId}\n");
//}

//app.MapPost("/webhook", async (Update update) =>
//{
//    if (update.Message != null)
//    {
//        // Handle start command or other messages
//        if (update.Message.Text.StartsWith("/start"))
//        {
//            await StartCommandHandler(update);
//        }
//        else
//        {
//            await SaveAddressHandler(update);
//        }
//    }
//    else if (update.CallbackQuery != null)
//    {
//        // Handle callback queries (e.g., change address or pay)
//        if (update.CallbackQuery.Data == "change_address")
//        {
//            // Handle the Change Address request
//            await ChangeAddressHandler(update);

//            // Send an answer to the callback query (acknowledge the button press)
//            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Please provide your new address:");
//        }
//        else if (update.CallbackQuery.Data == "pay")
//        {
//            // Handle the Pay request
//            await PayHandler(update);

//            // Send an answer to the callback query (acknowledge the button press)
//            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Proceeding with payment...");
//        }
//    }

//    return Results.Ok();
//});

//// Payment Callback Endpoints
//app.MapPost("/success", async (HttpRequest request) =>
//{
//    var paymentData = await new StreamReader(request.Body).ReadToEndAsync();
//    Console.WriteLine("Payment Success: " + paymentData);
//    return Results.Ok("Payment Success");
//});

//app.MapPost("/fail", async (HttpRequest request) =>
//{
//    var paymentData = await new StreamReader(request.Body).ReadToEndAsync();
//    Console.WriteLine("Payment Failed: " + paymentData);
//    return Results.Ok("Payment Failed");
//});

//app.MapPost("/result", async (HttpRequest request) =>
//{
//    var paymentData = await new StreamReader(request.Body).ReadToEndAsync();
//    Console.WriteLine("Payment Result: " + paymentData);
//    return Results.Ok("Payment Result");
//});



//app.Run();


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramWebhookBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Create the Telegram bot client using your bot's API key
            var botClient = new TelegramBotClient("7798238582:AAFlDOPBn1qmrG8adDcUIn8OjoorTh7iBlI"); // Replace with your bot's API key

            // Set the webhook URL to point to your deployed Azure Web App
            var webhookUrl = "https://telegram-bot-trash-payment.azurewebsites.net/api/telegram"; // Replace with your Azure Web App URL
            await botClient.SetWebhookAsync(webhookUrl);

            // Start the web application
            CreateHostBuilder(args, botClient).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, TelegramBotClient botClient) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services =>
                    {
                        // Add services required for the application
                        services.AddControllers();
                    })
                    .Configure(app =>
                    {
                        // Configure middleware for routing requests
                        app.UseRouting();

                        // Handle the incoming webhook requests at the /api/telegram route
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapPost("/api/telegram", async context =>
                            {
                                // Read incoming JSON payload (Webhook update)
                                var update = await context.Request.ReadFromJsonAsync<Update>();

                                if (update?.Message != null)
                                {
                                    var message = update.Message;

                                    // Handle /start command
                                    if (message.Text != null && message.Text.ToLower() == "/start")
                                    {
                                        var inlineKeyboard = new InlineKeyboardMarkup(
                                            new[]
                                            {
                                                new[] { InlineKeyboardButton.WithCallbackData("Click Me!", "lll") }
                                            });

                                        // Send a message with a button
                                        await botClient.SendTextMessageAsync(message.Chat.Id, "Welcome! Click the button below.", replyMarkup: inlineKeyboard);
                                    }
                                }

                                // Check for callback queries (button clicks)
                                if (update?.CallbackQuery != null)
                                {
                                    var callbackQuery = update.CallbackQuery;
                                    var userId = callbackQuery.From.Id;

                                    // Save the user ID to the addresses.txt file
                                    File.AppendAllText("addresses.txt", $"{userId}{System.Environment.NewLine}");

                                    // Acknowledge the callback with a response
                                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Your ID has been saved!");

                                    // Optionally, send a confirmation message to the user
                                    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Your Telegram ID has been saved to the file.");
                                }

                                // Respond with 200 OK to Telegram
                                context.Response.StatusCode = 200;
                            });
                        });
                    });
                });
    }
}
