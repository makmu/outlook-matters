using System.Xml;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace OutlookMatters.Test.TestUtils
{
    public static class ExtensionMethods
    {
        public static XmlAssertions WithNamespace(this StringAssertions assertions, string ns, string url)
        {
            return new XmlAssertions(assertions, ns, url);
        }

        public static void ContainXmlNode(this XmlAssertions assertion, string xpath, string because)
        {
            var doc = new XmlDocument();
            doc.LoadXml(assertion.Subject);
            var nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace(assertion.NamespaceKey, assertion.NamespaceUri);
            var node = doc.SelectSingleNode(xpath, nsManager);

            node.Should()
                .NotBeNull(because);
        }

        public static void DoNotContainXmlNode(this XmlAssertions assertion, string xpath, string because)
        {
            var doc = new XmlDocument();
            doc.LoadXml(assertion.Subject);
            var nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace(assertion.NamespaceKey, assertion.NamespaceUri);
            var node = doc.SelectSingleNode(xpath, nsManager);

            node.Should()
                .BeNull(because);
        }
    }
}