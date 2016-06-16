using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PS.Puzzle03
{
    public static class CommonHelper
    {
        public static string ParseXmlElement(XElement xelement)
        {
            if (xelement == null) return string.Empty;
            return xelement.Value;
        }
    }
}
