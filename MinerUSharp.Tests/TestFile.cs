using EasyReasy;

namespace MinerUSharp.Tests
{
    [ResourceCollection(typeof(EmbeddedResourceProvider))]
    internal static class TestFile
    {
        public static readonly Resource Image01 = new Resource("TestFiles/Image01.png");
        public static readonly Resource Text01 = new Resource("TestFiles/Text01.txt");
        public static readonly Resource MarkdownResponse01 = new Resource("TestFiles/MarkdownResponse01.txt");
        public static readonly Resource SimpleMarkdownResponse = new Resource("TestFiles/SimpleMarkdownResponse.txt");
        public static readonly Resource ClientTestResponse = new Resource("TestFiles/ClientTestResponse.txt");
        public static readonly Resource MultiResultResponse = new Resource("TestFiles/MultiResultResponse.txt");
    }
}
