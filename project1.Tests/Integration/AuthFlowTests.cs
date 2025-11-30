using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using project1.Application.DTOs.Auth;
using FluentAssertions;

namespace project1.Tests.Integration
{
    public class AuthFlowTests : IClassFixture<project1.Tests.TestWebFactory>
    {
        private readonly project1.Tests.TestWebFactory _factory;

        public AuthFlowTests(project1.Tests.TestWebFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Register_Login_AccessProtected()
        {
            var client = _factory.CreateClient();

            var reg = new RegisterRequest { Name = "IntTest Student", Email = "intstudent@school.test", Password = "P@ssw0rd1", Role = project1.Domain.Enums.Role.Student };
            var regResp = await client.PostAsJsonAsync("/api/auth/register", reg);
            regResp.EnsureSuccessStatusCode();

            var login = new LoginRequest { Email = "intstudent@school.test", Password = "P@ssw0rd1" };
            var loginResp = await client.PostAsJsonAsync("/api/auth/login", login);
            loginResp.EnsureSuccessStatusCode();

            var auth = await loginResp.Content.ReadFromJsonAsync<AuthResponse>();
            auth.Should().NotBeNull();
            auth!.AccessToken.Should().NotBeNullOrEmpty();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);

            var depResp = await client.GetAsync("/api/admin/departments");
            // student should be forbidden
            depResp.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }
    }
}
