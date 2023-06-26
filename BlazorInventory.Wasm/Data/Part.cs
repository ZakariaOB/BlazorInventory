using System.Web;
using System.Xml.Linq;

namespace BlazorInventory.Data
{
    public partial class Part
    {
        public string Url => $"https://letmebingthatforyou.com/?q={HttpUtility.UrlEncode(Name)}";
    }
}
