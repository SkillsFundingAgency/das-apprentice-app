using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.ApprenticeApp.Pwa.Helpers;
using System.Text.Json;

namespace SFA.DAS.ApprenticeApp.Pwa.UnitTests.Helpers
{
    public class TempDataExtensionsTests
    {
        private static TempDataDictionary CreateTempData()
        {
            var httpContext = new DefaultHttpContext();
            return new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        private record TestModel(string Name, int Value);

        [Test]
        public void Put_Should_Store_Serialized_Object_And_Get_Should_Return_It()
        {
            var tempData = CreateTempData();
            var model = new TestModel("abc", 123);

            tempData.Put("myKey", model);
            var result = tempData.Get<TestModel>("myKey");

            result.Should().NotBeNull();
            result!.Name.Should().Be(model.Name);
            result.Value.Should().Be(model.Value);
        }

        [Test]
        public void Get_Should_Return_Null_When_Key_Not_Present()
        {
            var tempData = CreateTempData();

            var result = tempData.Get<TestModel>("missing");

            result.Should().BeNull();
        }

        [Test]
        public void Get_Should_Return_Null_When_Stored_Value_Is_Invalid_Json()
        {
            var tempData = CreateTempData();
            tempData["bad"] = "not-a-json";

            var result = tempData.Get<TestModel>("bad");

            result.Should().BeNull();
        }

        [Test]
        public void Put_Should_Serialize_With_SystemTextJson()
        {
            var tempData = CreateTempData();
            var model = new TestModel("x", 1);

            tempData.Put("jsonKey", model);

            tempData.TryGetValue("jsonKey", out var stored);
            stored.Should().NotBeNull();
            var expected = JsonSerializer.Serialize(model);
            stored!.ToString().Should().Be(expected);
        }
    }
}