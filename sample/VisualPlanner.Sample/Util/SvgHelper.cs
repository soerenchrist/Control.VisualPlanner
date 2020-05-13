using System.IO;
using System.Reflection;

namespace VisualPlanner.Sample.Util
{
    public static class SvgHelper
    {
        public static string GetImageString(string svgName)
        {
            var type = typeof(SvgHelper).GetTypeInfo();
            var assembly = type.Assembly;

            var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Images.{svgName}");

            if (stream != null)
            {
                StreamReader reader = new StreamReader(stream);
                string text = reader.ReadToEnd();
                return text;
            }

            return "";
        }
    }
}