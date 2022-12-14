using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using Xunit;

namespace TaskListAPI.Test
{
    public class CustomerControllerTest : ControllerTest
    {
        public CustomerControllerTest(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }
        
        [Fact]
        public async void GetAll()
        {
            var client = Factory.CreateClient();
            await AddInitialClients(client);
            
            // valid get all
            var response = await client.GetAsync("customers");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
            var result = JArray.Parse(await response.Content.ReadAsStringAsync());
            Assert.Equal(2, result.Count);
            Assert.Equal(new HashSet<string> {"klant1", "klant2"},
                result.Select(node => node["name"].ToString()).ToHashSet());
        }
        
        [Fact]
        public async void Get()
        {
            var client = Factory.CreateClient();
            await AddInitialClients(client);
            
            var expected = new Dictionary<int, string>
            {
                {1, "klant1"},
                {2, "klant2"}
            };
        
            foreach (var keyval in expected)
            {
                // valid get
                var response = await client.GetAsync($"customers/{keyval.Key}");
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                
                var result = JObject.Parse(await response.Content.ReadAsStringAsync());
                Assert.NotNull(result);
                Assert.Equal(keyval.Value, result["name"]!.ToString());
            }
        }

        private async Task AddInitialClients(HttpClient client)
        {
            await client.PostAsync("customers", new StringContent(
                "{\n  \"name\" : \"klant1\"\n}", Encoding.UTF8, "application/json"));
            await client.PostAsync("customers", new StringContent(
                "{\n  \"name\" : \"klant2\"\n}", Encoding.UTF8, "application/json"));
        }
        
        [Fact]
        public async void GetNotFound()
        {
            var client = Factory.CreateClient();
            
            // get non-existent customer
            var response = await client.GetAsync($"customers/3");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async void PostAlreadyExists()
        {
            var client = Factory.CreateClient();
            await AddInitialClients(client);
            
            // violate unique name constraint
            var response = await client.PostAsync("customers", new StringContent(
                "{\n  \"name\" : \"klant1\"\n}", Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            var result = await response.Content.ReadAsStringAsync();
            Assert.NotNull(result);
            Assert.Contains("klant1 already exists", result);
        }
        
        [Fact]
        public async void PostEmptyContent()
        {
            var client = Factory.CreateClient();
            
            // no customer name specified
            var response = await client.PostAsync("customers", new StringContent(
                "{\n \n}", Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            var result = JObject.Parse(await response.Content.ReadAsStringAsync());
            Assert.NotNull(result);
            Assert.Contains("Name field is required", result["errors"]!.ToString());
        }
        
        [Fact]
        public async void PostInvalidContentUnknownKey()
        {
            var client = Factory.CreateClient();
            
            // invalid content specified
            var response = await client.PostAsync("customers", new StringContent(
                "{\n  \"name\" : \"test customer\",\n  \"asdf\" : \"asdf\"\n}", Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            var result = JObject.Parse(await response.Content.ReadAsStringAsync());
            Assert.NotNull(result);
            Assert.Contains("Could not find member 'asdf'", result["errors"]!.ToString());
        }
        
        [Fact]
        public async void PostInvalidContentEmptyName()
        {
            var client = Factory.CreateClient();
            
            // invalid content specified
            var response = await client.PostAsync("customers", new StringContent(
                "{\n  \"name\" : null\n}", Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
            var result = JObject.Parse(await response.Content.ReadAsStringAsync());
            Assert.NotNull(result);
            Assert.Contains("Name field is required", result["errors"]!.ToString());
        }
        
        [Fact]
        public async void DeleteDoesNotExist()
        {
            var client = Factory.CreateClient();
            
            // delete non-existent customer
            var response = await client.DeleteAsync($"customers/3");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [Fact]
        public async void PostAndDelete()
        {
            var client = Factory.CreateClient();
            
            // valid create
            var responseCreate = await client.PostAsync("customers", new StringContent(
                "{\n  \"name\" : \"test customer\"\n}", Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.Created, responseCreate.StatusCode);
        
            var result = JObject.Parse(await responseCreate.Content.ReadAsStringAsync());
            var id = result["id"]!.ToString();
            Assert.NotEmpty(id);
            Assert.Equal("test customer", result["name"]!.ToString());
        
            // valid delete
            var responseDelete = await client.DeleteAsync($"customers/{id}");
            Assert.Equal(HttpStatusCode.OK, responseDelete.StatusCode);
        }
    }
}