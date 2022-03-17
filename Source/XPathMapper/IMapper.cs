using System;
using System.Xml.XPath;

namespace Mapping
{
    public interface IMapper
    {
        object Map(Type type, IXPathNavigable document);
    }
}