﻿namespace Domain.Entities
{
    using System.Collections.Generic;

    using Domain.Common;

    using Shared.Exceptions;

    public class Color : ValueObject
    {
        private static readonly Dictionary<string, string> ColorNames = new Dictionary<string, string>
        {
            //{ "#FFFFFF", "White" },
            //{ "#FF5733", "Red" },
            //{ "#FFC300", "Orange" },
            //{ "#FFFF66", "Yellow" },
            //{ "#CCFF99", "Green" },
            //{ "#6666FF", "Blue" },
            //{ "#9966CC", "Purple" },
            //{ "#999999", "Grey" }

            { "rgb(255, 223, 61)", "Yellow" },
            { "rgb(255, 142, 36)", "Orange" },
            { "rgb(232, 67, 254)", "Purple" },
            { "rgb(36, 142, 255)", "Blue" },
            { "rgb(74, 222, 128)", "Green" },
            //{ "rgb(31, 255, 68)", "Green" },
        };

        static Color()
        {
        }

        private Color()
        {
        }

        private Color(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public static Color From(string code)
        {
            if (!ColorNames.TryGetValue(code, out var name))
            {
                throw new CustomException($"Unsupported color: {code}");
            }

            return new Color { Code = code, Name = name };
        }

        //public static Color White => new("#FFFFFF", "White");
        //public static Color Red => new("#FF5733", "Red");
        //public static Color Orange => new("#FFC300", "Orange");
        //public static Color Yellow => new("#FFFF66", "Yellow");
        //public static Color Green => new("#CCFF99", "Green");
        //public static Color Blue => new("#6666FF", "Blue");
        //public static Color Purple => new("#9966CC", "Purple");
        //public static Color Grey => new("#999999", "Grey");

        public static Color Yellow => new("rgb(255, 223, 61)", "Yellow");
        public static Color Orange => new("rgb(255, 142, 36)", "Orange");
        public static Color Purple => new("rgb(232, 67, 254)", "Purple");
        public static Color Blue => new("rgb(36, 142, 255)", "Blue");
        public static Color Green => new("rgb(74, 222, 128)", "Green");
        //public static Color Green => new("rgb(31, 255, 68)", "Green");

        public string Name { get; private set; }
        public string Code { get; private set; } = "#000000";

        public static implicit operator string(Color colour)
        {
            return colour.ToString();
        }

        public static explicit operator Color(string code)
        {
            return From(code);
        }

        public override string ToString()
        {
            return Code;
        }

        public static IEnumerable<Color> SupportedColors
        {
            get
            {
                //yield return White;
                //yield return Red;
                //yield return Orange;
                //yield return Yellow;
                //yield return Green;
                //yield return Blue;
                //yield return Purple;
                //yield return Grey;

                yield return Yellow;
                yield return Orange;
                yield return Purple;
                yield return Blue;
                yield return Green;
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
        }
    }
}