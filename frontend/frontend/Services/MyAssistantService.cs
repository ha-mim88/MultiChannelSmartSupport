using frontend.Data;
using frontend.Entities;
using frontend.Tools;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using OllamaSharp.Tools;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Chat;
using OpenAI.Responses;
using System;
using System.ClientModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using static OllamaSharp.Models.Chat.Message;

namespace frontend.Services
{
    public class MyAssistantService
    {
        private readonly ChatClient _client;
        private readonly IConfiguration _config;
        private readonly ILogger<MyAssistantService> _logger;
        private readonly Dictionary<string, string> myorderlist = new Dictionary<string, string>
            {
                ["123"] = "Shipped Nov 20, arriving tomorrow",
                ["124"] = "Delivered yesterday",
                ["125"] = "Processing, estimated ship date Dec 1",
                ["126"] = "Cancelled per user request",
                ["127"] = "Returned, refund issued",
                ["128"] = "Shipped Nov 18, delayed in transit",
                ["129"] = "Out for delivery today",
                ["130"] = "Processing payment, will ship soon",
                ["131"] = "On hold, contact support for details",
                ["132"] = "Delivered Nov 15",
                ["133"] = "Shipped Nov 19, arriving Nov 25",
                ["134"] = "Preparing for shipment",
                ["135"] = "Cancelled due to payment issue",
                ["136"] = "Returned to sender, address issue",
                ["137"] = "Shipped Nov 17, in transit",
                ["138"] = "Processing, estimated ship date Dec 3",
                ["139"] = "Delivered Nov 10",
                ["140"] = "Out for delivery Nov 22",
            };

    public MyAssistantService(IConfiguration config, ILogger<MyAssistantService> logger)
        {
            _config = config;
            _logger = logger;

            //var useLocal = config.GetValue<bool>("Llm:UseLocal", true);

            _client = new(
                model: "google/gemma-3-12b",
                credential: new ApiKeyCredential("lm-studio"),
                options: new OpenAIClientOptions()
                {
                    Endpoint = new Uri("http://localhost:1234/v1")
                }
            );
        }

        public async Task<string> ChatWithToolsAsync(
            List<AIChatHistory> history,
            string userMessage,
            int aiSessionId,
            ApplicationDbContext db)
        {
            // Save user message
            var userMsg = new AIChatHistory { AISessionId = aiSessionId, Role = "user", Content = userMessage, CreatedAt = DateTime.UtcNow, TokenUsage = 0 };
            db.AIChatHistory.Add(userMsg);
            await db.SaveChangesAsync();
            history.Add(userMsg);

            var session = await db.AISession.FindAsync(aiSessionId);
            var analytics = await db.ConversationAnalytics
                .FirstOrDefaultAsync(a => a.SessionId == aiSessionId);

            // Build full context
            var messages = new List<ChatMessage>();
            foreach (var item in history)
            {
                switch (item.Role)
                {
                    case "system":
                        messages.Add(new SystemChatMessage(item.Content));
                        break;
                    //case "developer":
                    //    messages.Add(new DeveloperChatMessage(item.Content));
                    //    break;
                    case "user":
                        messages.Add(new UserChatMessage(item.Content));
                        break;
                    case "assistant":
                        messages.Add(new AssistantChatMessage(item.Content));
                        break;
                    case "tool":
                        var dicts = JsonSerializer.Deserialize<Dictionary<string, object>[]>(item.Content);

                        foreach (var d in dicts)
                        {
                            var content = d.ContainsKey("content") ? d["content"]?.ToString() : null;

                            if (content != null)
                            {
                                messages.Add(new ToolChatMessage(content));
                            }
                        }
                        //messages.AddRange(JsonSerializer.Deserialize<ToolChatMessage[]>(item.Content));
                        break;
                    //case "function":
                    //    messages.Add(new FunctionChatMessage(item.Content));
                    //    break;
                    default:
                        break;
                }
            }

            //messages.Add(new UserChatMessage(userMessage));

            // 3. Define tools with NEW v2 API (no deprecations!)
            var getOrderStatus = ChatTool.CreateFunctionTool(
                    functionName: "check_order",
                    functionDescription: "Look up order status by order number. Use when user mentions order, tracking, delivery, etc.",
                    functionParameters: BinaryData.FromString(@"
                    {
                        ""type"": ""object"",
                        ""properties"": {
                            ""order_id"": {
                                ""type"": ""string"",
                                ""description"": ""The exact order number (e.g., '123', 'ORD-456')""
                            }
                        },
                        ""required"": [""order_id""]
                    }")
                );
            var getAllOrders = ChatTool.CreateFunctionTool(
                    functionName: "get_all_orders",
                    functionDescription: "Retrieve a list of all orders for the user. Use when user asks about their orders in general.",
                    functionParameters: BinaryData.FromString(@"
                    {
                        ""type"": ""object"",
                        ""properties"": {}
                    }")
                );

            // 4. Chat completion with tools
            var chatClient = new ChatClient(
                model: "google/gemma-3-12b",
                credential: new ApiKeyCredential("lm-studio"),
                options: new OpenAIClientOptions()
                {
                    Endpoint = new Uri("http://localhost:1234/v1")
                }
            );
            ChatCompletionOptions options = new ChatCompletionOptions
            {
                Tools = { getOrderStatus, getAllOrders }
            };

            try
            {
                var response = await chatClient.CompleteChatAsync(messages, options);  // For streaming: CompleteChatStreamingAsync

                // 5. Handle tool calls
                if (response.Value.ToolCalls?.Count > 0)
                {
                    var toolCallMessages = new List<ToolChatMessage>();
                    foreach (var toolCall in response.Value.ToolCalls)
                    {
                        switch (toolCall.FunctionName)
                        {
                            case "check_order":
                                {
                                    using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                                    bool hasOrderId = argumentsJson.RootElement.TryGetProperty("order_id", out JsonElement order_id);

                                    if (!hasOrderId)
                                    {
                                        throw new ArgumentNullException(nameof(order_id), "The order_id argument is required.");
                                    }

                                    string toolResult = CheckOrder(order_id.GetString());

                                    toolCallMessages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                    break;
                                }
                            case "get_all_orders":
                                {
                                    string toolResult = JsonSerializer.Serialize(GetAllOrders());
                                    toolCallMessages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                    break;
                                }
                            default:
                                {
                                    // Handle other unexpected calls.
                                    throw new NotImplementedException();
                                }
                        }
                    }

                    var toolDbMsg = new AIChatHistory
                    {
                        AISessionId = aiSessionId,
                        Role = "tool",
                        Content = JsonSerializer.Serialize(toolCallMessages),
                        TokenUsage = 0,
                        CreatedAt = DateTime.UtcNow
                    };

                    db.AIChatHistory.Add(toolDbMsg);
                    await db.SaveChangesAsync();

                    // Add tool result back to messages + re-call LLM for final response
                    if(response.Value.Content?[0]?.Text != null)
                        messages.Add(new AssistantChatMessage(response.Value.Content?[0]?.Text ?? ""));

                    messages.AddRange(toolCallMessages);

                    var finalResponse = await chatClient.CompleteChatAsync(messages, options);
                    var finalReply = finalResponse.Value.Content?[0]?.Text ?? "Tool executed successfully!";

                    // Persist final assistant reply
                    var assistantDbMsg = new AIChatHistory
                    {
                        AISessionId = aiSessionId,
                        Role = "assistant",
                        Content = finalReply,
                        TokenUsage = (response.Value.Usage?.TotalTokenCount ?? 0) + (finalResponse.Value.Usage?.TotalTokenCount ?? 0),
                        CreatedAt = DateTime.UtcNow
                    };
                    db.AIChatHistory.Add(assistantDbMsg);

                    #region analytics
                    if (analytics == null)
                    {
                        analytics = new ConversationAnalytics
                        {
                            SessionId = aiSessionId,
                            UserId = session.UserId,
                            StartedAt = DateTime.UtcNow,
                            MessageCount = 1,
                            ToolCalls = response.Value.ToolCalls.Count
                        };
                        db.ConversationAnalytics.Add(analytics);
                    }
                    else
                    {
                        analytics.MessageCount++;
                        analytics.ToolCalls += response.Value.ToolCalls.Count;
                        analytics.EndedAt = DateTime.UtcNow;
                    }
                    #endregion
                    await db.SaveChangesAsync();


                    return finalReply;
                }

                // No tool call → direct reply
                var directReply = response.Value.Content?[0]?.Text ?? "I got your message!";
                var assistantMsg = new AIChatHistory
                {
                    AISessionId = aiSessionId,
                    Role = "assistant",
                    Content = directReply,
                    TokenUsage = response.Value.Usage?.TotalTokenCount ?? 0,
                    CreatedAt = DateTime.UtcNow
                };
                db.AIChatHistory.Add(assistantMsg);

                #region analytics for direct reply
                if (analytics == null)
                {
                    analytics = new ConversationAnalytics
                    {
                        SessionId = aiSessionId,
                        UserId = session.UserId,
                        StartedAt = DateTime.UtcNow,
                        MessageCount = 1,
                        ToolCalls = response.Value.Usage?.TotalTokenCount ?? 0
                    };
                    db.ConversationAnalytics.Add(analytics);
                }
                else
                {
                    analytics.MessageCount++;
                    analytics.EndedAt = DateTime.UtcNow;
                }
                #endregion
                await db.SaveChangesAsync();

                return directReply;
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LLM chat failed for session {SessionId}", aiSessionId);
                return "Sorry, I'm having trouble right now. Try again?, "+ex.Message.ToString();
            }
        }

        private string CheckOrder(string orderId)
        {
            var res = new
            {
                status = myorderlist.ContainsKey(orderId) ? "found" : "not_found",
                order_id = orderId,
                message = myorderlist.GetValueOrDefault(orderId, "Not found — ticket created")
            };

            var options = new JsonSerializerOptions { WriteIndented = false };
            return JsonSerializer.Serialize(res, options);
        }
        // return all my orders
        private string GetAllOrders()
        {
            var res = myorderlist.ToList();
            var options = new JsonSerializerOptions { WriteIndented = false };
            return JsonSerializer.Serialize(res, options);
        }
    }
}
