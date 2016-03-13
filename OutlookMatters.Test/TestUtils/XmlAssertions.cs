using FluentAssertions.Primitives;

namespace OutlookMatters.Test.TestUtils
{
    public class XmlAssertions
    {
        public string NamespaceKey { get; }
        public string NamespaceUri { get; }
        public string Subject { get; }

        public XmlAssertions(StringAssertions stringAssertions, string namespaceKey, string namespaceUri)
        {
            NamespaceKey = namespaceKey;
            NamespaceUri = namespaceUri;
            Subject = stringAssertions.Subject;
        }
    }
}