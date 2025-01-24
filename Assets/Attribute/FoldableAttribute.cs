using UnityEngine;

namespace Mekaiju.Attributes
{
    public class FoldableAttribute : PropertyAttribute
    {
        public string Header;

        public FoldableAttribute(string header)
        {
            Header = header;
        }
    }
}
