using Api.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test
{
    public class GeoMessageControllerTests : IClassFixture<WebApplicationFactory<Api.Startup>>
    {
        private readonly HttpClient _client;

        public GeoMessageControllerTests(WebApplicationFactory<Api.Startup> fixture)
        {
            _client = fixture.CreateClient();
        }

        [Fact]
        public async Task GetV1()
        {
            var response = await _client.GetAsync("api/v1/BerrasGeoApp/Get");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var geoMessages = JsonConvert.DeserializeObject<List<GeoMessageDto>>(await response.Content.ReadAsStringAsync());
            geoMessages.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetWithinRange()
        {
            double minLon = 0;
            double maxLon = 10;
            double minLat = 0;
            double maxLat = 10;

            var response = await _client.GetAsync($"api/v2/BerrasGeoApp/Get?maxLon={maxLon}&maxLat={maxLat}");
            var geoMessages = JsonConvert.DeserializeObject<List<GeoMessageV2Dto>>(await response.Content.ReadAsStringAsync());

            geoMessages.Select(g => g.Latitude).Max().Should().BeLessThan(maxLat);
            geoMessages.Select(g => g.Longitude).Max().Should().BeLessThan(maxLon);
            geoMessages.Select(g => g.Latitude).Min().Should().BeGreaterThan(minLat);
            geoMessages.Select(g => g.Longitude).Min().Should().BeGreaterThan(minLon);
        }

        [Fact]
        public async Task PostV2ShouldReturnAuthor()
        {

            await Task.CompletedTask;
        }
    }
}
