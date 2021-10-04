using System;
using System.Drawing;
using System.Linq;

namespace LightingControl
{
    public class Utils
    {
        public static Color ParseColor(string type, string color)
        {
            Color s;
            switch (type.ToLower())
            {
                case "rgb":
                    var rgb = color.Split(',').ToList().ConvertAll<int>(col =>
                    {
                        var res = int.TryParse(col, out int colorVal);
                        if (!res) throw new ArgumentException("RGB type expects only 3 comma seperated integers!");
                        return colorVal;
                    }).ToArray();
                    if (rgb.Length != 3)
                    {
                        throw new ArgumentException("RGB type expects only 3 comma seperated integers!");
                    }

                    s = Color.FromArgb(rgb[0], rgb[1], rgb[2]);
                    break;
                case "name":
                    s = Color.FromName(color);
                    break;
                default:
                    throw new ArgumentException("Color type (--type) must be either rgb or name!");
            }

            return s;
        }
    }
}