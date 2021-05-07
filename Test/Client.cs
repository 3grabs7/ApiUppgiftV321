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
        public readonly HttpClient _client;

        public GeoMessageControllerTests(WebApplicationFactory<Api.Startup> fixture)
        {
            _client = fixture.CreateClient();
        }

        [Fact]
        public async Task Get()
        {
            var response = await _client.GetAsync("api/v1/BerrasGeoApp/Get");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var geoMessages = JsonConvert.DeserializeObject<List<GeoMessageV2Dto>>(await response.Content.ReadAsStringAsync());
            geoMessages.Should().HaveCount(1);
        }
        [Fact]
        public async Task GetWithinRange()
        {


        }
    }
}
