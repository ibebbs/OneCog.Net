using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Utility;
using NUnit.Framework;

namespace OneCog.Net.Instrumentation.Tests
{
    [TestFixture]
    public class EventSourceTests
    {
        [Test]
        public void ConnectionEventSourceIsValid()
        {
            EventSourceAnalyzer analyzer = new EventSourceAnalyzer();

            analyzer.Inspect(Connection.Log);
        }
    }
}
