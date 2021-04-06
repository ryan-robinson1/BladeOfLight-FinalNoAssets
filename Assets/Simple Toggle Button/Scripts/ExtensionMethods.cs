using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleToggleButton.Extensions
{
    public static class ExtensionMethods
    {
        public static Color WithOpacityOf(this Color source, Color target)
        {
            return new Color(source.r, source.g, source.b, target.a);
        }

        public static Color WithOpacityOf(this Color source, float opacity)
        {
            return new Color(source.r, source.g, source.b, opacity);
        }
    }
}
