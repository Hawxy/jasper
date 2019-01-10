using Jasper.Http;
using Shouldly;
using Xunit;

namespace Jasper.Testing.Http
{
    public class HttpSettingsTests
    {
        [Fact]
        public void default_compliance_mode_is_full()
        {
            new HttpSettings().AspNetCoreCompliance.ShouldBe(ComplianceMode.FullyCompliant);
        }
    }
}
