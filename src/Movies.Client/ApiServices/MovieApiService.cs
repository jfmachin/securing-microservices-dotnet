using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Movies.Client.Extensions;
using Movies.Client.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace Movies.Client.ApiServices {
    public class MovieApiService : IMovieApiService {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IHttpContextAccessor httpContextAccessor;

        public MovieApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) {
            this.httpClientFactory = httpClientFactory;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Movie> CreateMovie(Movie movie) {
            var serializedMovie = JsonConvert.SerializeObject(movie);
            var httpClient = httpClientFactory.CreateClient("MovieAPIClient");
            var request = new HttpRequestMessage(HttpMethod.Post, "/movies");

            request.Content = new StringContent(serializedMovie);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            return await response.ReadContentAs<Movie>();
        }

        public async Task<Movie> DeleteMovie(int id) {
            var httpClient = httpClientFactory.CreateClient("MovieAPIClient");
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/movies/{id}");
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            return await response.ReadContentAs<Movie>();
        }

        public async Task<Movie> GetMovie(int id) {
            var httpClient = httpClientFactory.CreateClient("MovieAPIClient");
            var request = new HttpRequestMessage(HttpMethod.Get, $"/movies/{id}");
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound) 
                return null;

            response.EnsureSuccessStatusCode();
            return await response.ReadContentAs<Movie>();
        }

        public async Task<IEnumerable<Movie>> GetMovies() {
            var httpClient = httpClientFactory.CreateClient("MovieAPIClient");
            var request = new HttpRequestMessage(HttpMethod.Get, "/movies");
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            return await response.ReadContentAs<List<Movie>>();
        }

        public async Task<IEnumerable<Movie>> GetMoviesByOwnerName(string name) {
            var httpClient = httpClientFactory.CreateClient("MovieAPIClient");
            var request = new HttpRequestMessage(HttpMethod.Get, $"/movies/{name}");
            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            return await response.ReadContentAs<List<Movie>>();
        }

        private object HttpClient()
        {
            throw new NotImplementedException();
        }

        public Task<Movie> UpdateMovie(Movie movie) {
            throw new NotImplementedException();
        }

        public async Task<UserInfoViewModel> GetUserInfo() {
            var httpClient = httpClientFactory.CreateClient("IDPClient");
            var metaData = await httpClient.GetDiscoveryDocumentAsync();

            if (metaData.IsError)
                throw new HttpRequestException("Something went wrong while requesting the access token");

            var token = await httpContextAccessor.HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var userInfo = await httpClient.GetUserInfoAsync(
                new UserInfoRequest { 
                    Address = metaData.UserInfoEndpoint,
                    Token = token
                });

            if (userInfo.IsError)
                throw new HttpRequestException("Something went wrong while retrieving user info");

            var userInfoDict = new Dictionary<string, string>();
            foreach (var claim in userInfo.Claims)
                userInfoDict.Add(claim.Type, claim.Value);

            return new UserInfoViewModel(userInfoDict);
        }
    }
}
