using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using CSharpFunctionalExtensions;
using Mapping;
using Reflection;

namespace XPath
{
    public static class XmlMappingMixin
    {
        public static void Apply<T>(this MemberMap mm, T obj, IXPathNavigable document, Maybe<IMapper> xPathMapper)
        {
            var reflectedMember = ReflectedMember.From(obj, mm.Member);

            if (reflectedMember.IsCollection)
            {
                if (xPathMapper.HasNoValue)
                {
                    throw new InvalidOperationException("We need a mapper to map collections");
                }

                ApplyToCollection(mm, document, xPathMapper.Value, reflectedMember.AsCollection().Value);
            }
            else
            {
                var processed = mm.Call(s => GetXpathValue(s, document));

                if (processed.GetType().IsAssignableTo(reflectedMember.MemberType))
                {
                    reflectedMember.Set(processed);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Cannot assign value of type {processed.GetType()} to member {reflectedMember}");
                }
            }
        }

        private static void ApplyToCollection(MemberMap tm, IXPathNavigable document, IMapper xPathMapper, ReflectedCollection reflectedCollection)
        {
            var childrenRoot = tm.Sources.First();
            var children = GetChildren(document, childrenRoot).ToList();

            foreach (var child in children)
            {
                var mapped = xPathMapper.Map(reflectedCollection.ItemType, child);
                reflectedCollection.Add(mapped);
            }
        }

        private static IEnumerable<XPathNavigator> GetChildren(IXPathNavigable document, string childrenRoot)
        {
            var navigator = document.CreateNavigator();
            var nodes = navigator.Select(childrenRoot);
            while (nodes.MoveNext())
            {
                yield return nodes.Current.Clone();
            }
        }

        private static Maybe<string> GetXpathValue(string xpath, IXPathNavigable document)
        {
            var nav = document.CreateNavigator();
            var selectSingleNode = nav.SelectSingleNode(xpath);
            if (selectSingleNode != null)
            {
                return selectSingleNode.Value;
            }

            return Maybe.None;
        }
    }
}