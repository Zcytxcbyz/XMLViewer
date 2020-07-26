using System.Collections.Generic;
using System.Xml;

namespace System.XMLParser
{
    public class XMLAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class XMLObject
    {
        public string NodeName { get; set; }
        public string InnerXml { get; set; }
        public string InnerText { get; set; }
        public string Value { get; set; }
        public List<XMLAttribute> Attributes { get; set; }
        public List<XMLObject> ChildrenNode { get; set; }
    }
    public class XMLParser
    {
        public static XMLObject Parser(string XmlString)
        {
            XmlDocument docmunet = new XmlDocument();
            docmunet.LoadXml(XmlString);
            return Parser(docmunet);
        }
        public static XMLObject Parser(XmlDocument docmunet)
        {
            XmlElement root = docmunet.DocumentElement;
            if (root != null)
            {
                return Parser(root);
            }
            else
            {
                return new XMLObject
                {
                    ChildrenNode = Parser(docmunet.ChildNodes)
                };
            }
        }
        private static List<XMLObject> Parser(XmlNodeList nodeList)
        {
            List<XMLObject> xmlObjList = new List<XMLObject>();
            foreach(XmlNode node in nodeList)
            {
                if (node.NodeType != XmlNodeType.Text)
                    xmlObjList.Add(Parser(node));
            }
            return xmlObjList;
        }
        private static XMLObject Parser(XmlNode node)
        {
            //if (node.NodeType == XmlNodeType.Text) return null;
            List<XMLAttribute> attributes = new List<XMLAttribute>();
            if (node.Attributes != null)
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    attributes.Add(new XMLAttribute
                    {
                        Name = attribute.Name,
                        Value = attribute.Value
                    });
                }
            }
            return new XMLObject
            {
                NodeName = node.Name,
                InnerXml = node.InnerXml,
                InnerText = node.InnerText,
                Value = node.Value,
                Attributes = attributes,
                ChildrenNode = Parser(node.ChildNodes)
            };
        }
    }
}
