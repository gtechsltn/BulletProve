﻿using Example.Api.Controllers;

namespace Example.Api.IntegrationTests.Controllers
{
    /// <summary>
    /// The auth controller tests.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AuthControllerTests"/> class.
    /// </remarks>
    /// <param name="serverManager">The server manager.</param>
    /// <param name="output">The output.</param>
    public class AuthControllerTests(ServerManager serverManager, ITestOutputHelper output)
        : TestBase(serverManager, output)
    {
        /// <summary>
        /// Whens the credentials are ok then token returned.
        /// </summary>
        [Fact]
        public async Task WhenCredentialsAreOk_ThenTokenReturned()
        {
            var token = await LoginAsAdminAndGetTokenAsync(Server);

            using var response = await Server
                .HttpRequestFor<UserController>(x => x.GetUserDataAsync)
                .WithToken(token)
                .ExecuteSuccessAsync();
        }

        /// <summary>
        /// Whens the credentials are bad then unauthorized result.
        /// </summary>
        [Fact]
        public async Task WhenCredentialsAreBad_ThenUnauthorizedResult()
        {
            using var response = await Server
                .HttpRequestFor<AuthController>(x => x.LoginAsync)
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = TestConstants.AdminUsername,
                    Password = TestConstants.AdminBadPassword
                })
                .AddAllowedServerLogEvent(x => x.Message == "Wrong username or password")
                .ExecuteAssertingStatusAsync(HttpStatusCode.Unauthorized);

            response.Should().NotBeNull();
        }

        /// <summary>
        /// Whens the user is not logged in then unauthorized returned.
        /// </summary>
        [Fact]
        public async Task WhenUserIsNotLoggedIn_ThenUnauthorizedReturned()
        {
            using var response = await Server
                .HttpRequestFor<UserController>(x => x.GetUserDataAsync)
                .ExecuteAssertingStatusAsync(HttpStatusCode.Unauthorized);

            response.Should().NotBeNull();
        }
    }
}
