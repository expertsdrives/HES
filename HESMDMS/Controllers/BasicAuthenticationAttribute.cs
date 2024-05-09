using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace HESMDMS.Controllers
{
    public class BasicAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }
        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (authorization == null || authorization.Scheme != "Basic")
            {
                return;
            }

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
                return;
            }

            var usernamePasswordArray = ExtractUsernameAndPassword(authorization.Parameter);
            var username = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];

            // Replace this with your own validation logic
            if (username == "adani" && password == "Adani@2024#")
            {
                var identity = new GenericIdentity(username);
                var principal = new GenericPrincipal(identity, null);
                context.Principal = principal;
            }
            else
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid username or password", request);
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.Result = new AddChallengeOnUnauthorizedResult(new AuthenticationHeaderValue("Basic", $"realm=\"{Realm}\""), context.Result);
            return Task.FromResult(0);
        }

        private string[] ExtractUsernameAndPassword(string authorizationParameter)
        {
            var credentialBytes = Convert.FromBase64String(authorizationParameter);
            var credentials = Encoding.ASCII.GetString(credentialBytes).Split(':');
            if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[1]))
            {
                return null;
            }
            return credentials;
        }

        public class AuthenticationFailureResult : IHttpActionResult
        {
            public string ReasonPhrase { get; }
            public HttpRequestMessage Request { get; }

            public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
            {
                ReasonPhrase = reasonPhrase;
                Request = request;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(Execute());
            }

            private HttpResponseMessage Execute()
            {
                HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                response.RequestMessage = Request;
                response.ReasonPhrase = ReasonPhrase;
                return response;
            }
        }

        public class AddChallengeOnUnauthorizedResult : IHttpActionResult
        {
            public AuthenticationHeaderValue Challenge { get; }
            public IHttpActionResult InnerResult { get; }

            public AddChallengeOnUnauthorizedResult(AuthenticationHeaderValue challenge, IHttpActionResult innerResult)
            {
                Challenge = challenge;
                InnerResult = innerResult;
            }

            public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                var response = await InnerResult.ExecuteAsync(cancellationToken);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (!response.Headers.Contains("WWW-Authenticate"))
                    {
                        response.Headers.Add("WWW-Authenticate", Challenge.ToString());
                    }
                }
                return response;
            }
        }
    }
}