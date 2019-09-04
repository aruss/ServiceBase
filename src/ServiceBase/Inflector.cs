// Copyright (c) Russlan Akiev. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace ServiceBase
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public static class Inflector
    {
        private static readonly List<Rule> Plurals = new List<Rule>();
        private static readonly List<Rule> Singulars = new List<Rule>();
        private static readonly List<string> Uncountables = new List<string>();

        #region Default Rules

        static Inflector()
        {
            AddPlural("$", "s");
            AddPlural("s$", "s");
            AddPlural("(ax|test)is$", "$1es");
            AddPlural("(octop|vir|alumn|fung)us$", "$1i");
            AddPlural("(alias|status)$", "$1es");
            AddPlural("(bu)s$", "$1ses");
            AddPlural("(buffal|tomat|volcan)o$", "$1oes");
            AddPlural("([ti])um$", "$1a");
            AddPlural("sis$", "ses");
            AddPlural("(?:([^f])fe|([lr])f)$", "$1$2ves");
            AddPlural("(hive)$", "$1s");
            AddPlural("([^aeiouy]|qu)y$", "$1ies");
            AddPlural("(x|ch|ss|sh)$", "$1es");
            AddPlural("(matr|vert|ind)ix|ex$", "$1ices");
            AddPlural("([m|l])ouse$", "$1ice");
            AddPlural("^(ox)$", "$1en");
            AddPlural("(quiz)$", "$1zes");

            AddSingular("s$", "");
            AddSingular("(n)ews$", "$1ews");
            AddSingular("([ti])a$", "$1um");
            AddSingular("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
            AddSingular("(^analy)ses$", "$1sis");
            AddSingular("([^f])ves$", "$1fe");
            AddSingular("(hive)s$", "$1");
            AddSingular("(tive)s$", "$1");
            AddSingular("([lr])ves$", "$1f");
            AddSingular("([^aeiouy]|qu)ies$", "$1y");
            AddSingular("(s)eries$", "$1eries");
            AddSingular("(m)ovies$", "$1ovie");
            AddSingular("(x|ch|ss|sh)es$", "$1");
            AddSingular("([m|l])ice$", "$1ouse");
            AddSingular("(bus)es$", "$1");
            AddSingular("(o)es$", "$1");
            AddSingular("(shoe)s$", "$1");
            AddSingular("(cris|ax|test)es$", "$1is");
            AddSingular("(octop|vir|alumn|fung)i$", "$1us");
            AddSingular("(alias|status)es$", "$1");
            AddSingular("^(ox)en", "$1");
            AddSingular("(vert|ind)ices$", "$1ex");
            AddSingular("(matr)ices$", "$1ix");
            AddSingular("(quiz)zes$", "$1");

            AddIrregular("person", "people");
            AddIrregular("man", "men");
            AddIrregular("child", "children");
            AddIrregular("sex", "sexes");
            AddIrregular("move", "moves");
            AddIrregular("goose", "geese");
            AddIrregular("alumna", "alumnae");

            AddUncountable("equipment");
            AddUncountable("information");
            AddUncountable("rice");
            AddUncountable("money");
            AddUncountable("species");
            AddUncountable("series");
            AddUncountable("fish");
            AddUncountable("sheep");
            AddUncountable("deer");
            AddUncountable("aircraft");
        }

        #endregion Default Rules

        private class Rule
        {
            private readonly Regex regex;
            private readonly string replacement;

            public Rule(string pattern, string replacement)
            {
                this.regex = new Regex(pattern, RegexOptions.IgnoreCase);
                this.replacement = replacement;
            }

            public string Apply(string word)
            {
                return !this.regex.IsMatch(word) ?
                    null :
                    this.regex.Replace(word, this.replacement);
            }
        }

        private static void AddIrregular(string singular, string plural)
        {
            Inflector.AddPlural("(" + singular[0] + ")" + singular
                .Substring(1) + "$", "$1" + plural.Substring(1));

            Inflector.AddSingular("(" + plural[0] + ")" + plural
                .Substring(1) + "$", "$1" + singular.Substring(1));
        }

        private static void AddUncountable(string word)
        {
            Inflector.Uncountables.Add(word.ToLower());
        }

        private static void AddPlural(string rule, string replacement)
        {
            Inflector.Plurals.Add(new Rule(rule, replacement));
        }

        private static void AddSingular(string rule, string replacement)
        {
            Inflector.Singulars.Add(new Rule(rule, replacement));
        }

        public static string Pluralize(this string word)
        {
            return Inflector.ApplyRules(Inflector.Plurals, word);
        }

        public static string Singularize(this string word)
        {
            return Inflector.ApplyRules(Inflector.Singulars, word);
        }

        private static string ApplyRules(List<Rule> rules, string word)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            if (word == null)
            {
                throw new ArgumentNullException(nameof(word));
            }

            string result = word;

            if (!Inflector.Uncountables.Contains(word.ToLower()))
            {
                for (int i = rules.Count - 1; i >= 0; i--)
                {
                    if ((result = rules[i].Apply(word)) != null)
                    {
                        break;
                    }
                }
            }

            return result ?? word;
        }

        public static string Titleize(this string word)
        {
            return Regex.Replace(Inflector.Humanize(
                Underscore(word)), @"\b([a-z])",
                match => match.Captures[0].Value.ToUpper());
        }
        
        public static string Pascalize(this string lowercaseAndUnderscoredWord)
        {
            return Regex.Replace(lowercaseAndUnderscoredWord, "(?:^|_)(.)",
                match => match.Groups[1].Value.ToUpper());
        }

        public static string Camelize(this string lowercaseAndUnderscoredWord)
        {
            return Inflector.Uncapitalize(
                Inflector.Pascalize(lowercaseAndUnderscoredWord));
        }

        public static string Underscore(this string pascalCasedWord)
        {
            return Regex.Replace(
                Regex.Replace(
                    Regex.Replace(pascalCasedWord,
                    @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])",
                    "$1_$2"), @"[-\s]", "_").ToLower();
        }

        public static string Capitalize(this string word)
        {
            return word.Substring(0, 1).ToUpper() +
                word.Substring(1).ToLower();
        }

        public static string Uncapitalize(this string word)
        {
            return word.Substring(0, 1).ToLower() + word.Substring(1);
        }

        public static string Ordinalize(this string numberString)
        {
            return Inflector.Ordanize(int.Parse(numberString), numberString);
        }

        public static string Ordinalize(this int number)
        {
            return Inflector.Ordanize(number, number.ToString());
        }

        private static string Ordanize(int number, string numberString)
        {
            var nMod100 = number % 100;

            if (nMod100 >= 11 && nMod100 <= 13)
            {
                return numberString + "th";
            }

            switch (number % 10)
            {
                case 1:
                    return numberString + "st";

                case 2:
                    return numberString + "nd";

                case 3:
                    return numberString + "rd";

                default:
                    return numberString + "th";
            }
        }

        public static string Dasherize(this string underscoredWord)
        {
            return underscoredWord.Replace('_', '-');
        }
        
        public static string Humanize(this string input)
        {
            return Capitalize(HumanizeUnderscored(input));
        }

        public static string HumanizeDashed(this string input)
        {
            return Regex.Replace(input, @"-", " ");
        }

        public static string HumanizeUnderscored(this string input)
        {
            return Regex.Replace(input, @"_", " ");
        }

        public static string HumanizeCamelCased(this string input)
        {
            return Regex.Replace(input,
                "((?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z]))", " $1")
                .Trim()
                .ToLowerInvariant(); 

        }
    }
}