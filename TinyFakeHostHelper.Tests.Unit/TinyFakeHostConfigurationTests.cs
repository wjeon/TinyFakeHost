using NUnit.Framework;
using TinyFakeHostHelper.Configuration;
using TinyFakeHostHelper.Tests.Unit.Helpers;

namespace TinyFakeHostHelper.Tests.Unit
{
    [TestFixture]
    public class TinyFakeHostConfigurationTests
    {
        [Test]
        public void When_max_number_of_url_path_segments_is_configured_MaximumNumberOfUrlPathSegments_returns_configured_number()
        {
            AppSettingHelper.AddAppSettingInMemory("MaximumNumberOfPathSegments", "15");

            var tinyFakeHostConfiguration = new TinyFakeHostConfiguration();

            Assert.AreEqual(15, tinyFakeHostConfiguration.MaximumNumberOfUrlPathSegments);

            AppSettingHelper.RemoveAppSettingInMemory("MaximumNumberOfPathSegments");
        }

        [Test]
        public void When_max_number_of_url_path_segments_is_not_configured_MaximumNumberOfUrlPathSegments_returns_default_number_10()
        {
            AppSettingHelper.RemoveAppSettingInMemory("MaximumNumberOfPathSegments");

            var tinyFakeHostConfiguration = new TinyFakeHostConfiguration();

            Assert.AreEqual(10, tinyFakeHostConfiguration.MaximumNumberOfUrlPathSegments);
        }
    }
}
