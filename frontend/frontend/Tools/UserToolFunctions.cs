using frontend.Extension;
using frontend.Services;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace frontend.Tools
{
    public static class UserToolFunctions
    {
        public static async Task<string> GetUsers(string? searchUserName, int? top)
        {
            using (var scope = ServiceLocator.Provider.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var userList = await userService.GetUsers(searchUserName, top ?? 10);
                var options = new JsonSerializerOptions { WriteIndented = false, ReferenceHandler = ReferenceHandler.IgnoreCycles };
                return JsonSerializer.Serialize(userList, options);
            }
        }
        // Define the ChatTool for GetUsers function

        public static readonly ChatTool GetUserListTool = ChatTool.CreateFunctionTool(
            functionName: nameof(GetUsers),
            functionDescription: "Get a list of users",
            functionParameters: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "searchUserName": {
                            "type": "string",
                            "description": "The username to search for"
                        },
                        "top": {
                            "type": "integer",
                            "description": "The maximum number of users to return, default is 10"
                        }
                    },
                    "required": []
                }
                """u8.ToArray()
            )
        );


        // get user count
        public static async Task<string> GetUserCount()
        {
            using (var scope = ServiceLocator.Provider.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var count = userService.GetTotalUserCount();
                var options = new JsonSerializerOptions { WriteIndented = false };
                return JsonSerializer.Serialize(count, options);
            }
        }
        // Define the ChatTool for GetUsers function
        public static readonly ChatTool GetUserCountTool = ChatTool.CreateFunctionTool(
            functionName: nameof(GetUserCount),
            functionDescription: "Get total user count",
            functionParameters: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                    },
                    "required": []
                }
                """u8.ToArray()
            )
        );

        // get user by username
        public static async Task<string> GetSingleUser(string userName)
        {
            using (var scope = ServiceLocator.Provider.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var res = userService.GetSingleUser(userName);
                var options = new JsonSerializerOptions { WriteIndented = false };
                return JsonSerializer.Serialize(res, options);
            }
        }
        // Define the ChatTool for GetSingleUser function
        public static readonly ChatTool GetSingleUserTool = ChatTool.CreateFunctionTool(
            functionName: nameof(GetSingleUser),
            functionDescription: "Get a single user by username",
            functionParameters: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "userName": {
                            "type": "string",
                            "description": "The username of the user to retrieve"
                        }
                    },
                    "required": ["userName"]
                }
                """u8.ToArray()
            )
        );

    }
}
