using System;
using System.Xml.XPath;

namespace Zafiro.Mapping
{
    public interface IMapper
    {
        object Map(Type type, IXPathNavigable document);
    }
}