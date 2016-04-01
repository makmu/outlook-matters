using FluentAssertions.Primitives;

namespace OutlookMatters.Test.TestUtils
{
    public class XmlAssertions
    {
        public string NamespaceKey { get; private set; }
        public string NamespaceUri { get; private set; }
        public string Subject { get; private set; }

        public XmlAssertions(StringAssertions stringAssertions, string namespaceKey, string namespaceUri)
        {
            NamespaceKey = namespaceKey;
            NamespaceUri = namespaceUri;
            Subject = stringAssertions.Subject;
        }
    }
}