using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GameCreator.Editor.Search
{
    internal class PipelineStemmer : IPipeline
    {
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public string Run(string term)
        {
            if (term.Length < 3) return term;
            
            char firstCharacter = term[0];
            if (firstCharacter == 'y')
            {
                term = char.ToUpper(firstCharacter, Culture) + term[1..];
            }
            
            Regex re1 = RX1_1a;
            Regex re2 = RX2_1a;

            if (re1.IsMatch(term))
            {
                term = re1.Replace(term, "$1$2");
            }
            else if (re2.IsMatch(term))
            {
                term = re2.Replace(term, "$1$2");
            }
            
            re1 = RX1_1b;
            re2 = RX2_1b;
            if (re1.IsMatch(term))
            {
                GroupCollection fp = re1.Match(term).Groups;
                re1 = RX_mgr0;
                if (re1.IsMatch(fp[1].Value))
                {
                    re1 = RX1_1b_2;
                    term = re1.Replace(term, "");
                }
            }
            else if (re2.IsMatch(term))
            {
                GroupCollection fp = re2.Match(term).Groups;
                string stem = fp[1].Value;
                re2 = RX_s_v;
                if (re2.IsMatch(stem))
                {
                    term = stem;
                    re2 = RX2_1b_2;
                    Regex re3 = RX3_1b_2;
                    Regex re4 = RX4_1b_2;
                    if (re2.IsMatch(term))
                    {
                        term += "e";
                    }
                    else if (re3.IsMatch(term))
                    {
                        re1 = RX1_1b_2;
                        term = re1.Replace(term, "");
                    }
                    else if (re4.IsMatch(term))
                    {
                        term += "e";
                    }
                }
            }
            
            re1 = RX1_1c;
            if (re1.IsMatch(term))
            {
                GroupCollection fp = re1.Match(term).Groups;
                string stem = fp[1].Value;
                term = stem + "i";
            }
            
            re1 = RX1_2;
            if (re1.IsMatch(term))
            {
                GroupCollection fp = re1.Match(term).Groups;
                string stem = fp[1].Value;
                string suffix = fp[2].Value;
                re1 = RX_mgr0;
                if (re1.IsMatch(stem))
                {
                    term = stem + step2list[suffix];
                }
            }
            
            re1 = RX1_3;
            if (re1.IsMatch(term))
            {
                GroupCollection fp = re1.Match(term).Groups;
                string stem = fp[1].Value;
                string suffix = fp[2].Value;
                re1 = RX_mgr0;
                if (re1.IsMatch(stem))
                {
                    term = stem + step3list[suffix];
                }
            }
            
            re1 = RX1_4;
            re2 = RX2_4;
            if (re1.IsMatch(term))
            {
                GroupCollection fp = re1.Match(term).Groups;
                string stem = fp[1].Value;
                re1 = RX_mgr1;
                if (re1.IsMatch(stem))
                {
                    term = stem;
                }
            }
            else if (re2.IsMatch(term))
            {
                GroupCollection fp = re2.Match(term).Groups;
                string stem = fp[1].Value + fp[2].Value;
                re2 = RX_mgr1;
                if (re2.IsMatch(stem))
                {
                    term = stem;
                }
            }
            
            re1 = RX1_5;
            if (re1.IsMatch(term))
            {
                GroupCollection fp = re1.Match(term).Groups;
                string stem = fp[1].Value;
                re1 = RX_mgr1;
                re2 = RX_meq1;
                Regex re3 = RX3_5;
                if (re1.IsMatch(stem) || (re2.IsMatch(stem) && !re3.IsMatch(stem)))
                {
                    term = stem;
                }
            }

            re1 = RX1_5_1;
            re2 = RX_mgr1;
            if (re1.IsMatch(term) && re2.IsMatch(term))
            {
                re1 = RX1_1b_2;
                term = re1.Replace(term, "");
            }
            
            if (firstCharacter == 'y')
            {
                term = char.ToLower(firstCharacter, Culture) + term[1..];
            }

            return term;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // PORTER STEMMER (V1): -------------------------------------------------------------------

        private static readonly CultureInfo Culture = CultureInfo.CreateSpecificCulture("en");

        private static readonly Dictionary<string, string> step2list = new Dictionary<string, string>
        {
            {"ational", "ate"},
            {"tional", "tion"},
            {"enci", "ence"},
            {"anci", "ance"},
            {"izer", "ize"},
            {"bli", "ble"},
            {"alli", "al"},
            {"entli", "ent"},
            {"eli", "e"},
            {"ousli", "ous"},
            {"ization", "ize"},
            {"ation", "ate"},
            {"ator", "ate"},
            {"alism", "al"},
            {"iveness", "ive"},
            {"fulness", "ful"},
            {"ousness", "ous"},
            {"aliti", "al"},
            {"iviti", "ive"},
            {"biliti", "ble"},
            {"logi", "log"}
        };

        private static readonly Dictionary<string, string> step3list = new Dictionary<string, string>
        {
            {"icate", "ic"},
            {"ative", ""},
            {"alize", "al"},
            {"iciti", "ic"},
            {"ical", "ic"},
            {"ful", ""},
            {"ness", ""}
        };

        private const string Consonant = "[^aeiou]";
        private const string Vowel = "[aeiouy]";
        private const string ConsonantSequence = Consonant + "[^aeiouy]*";
        private const string VowelSequence = Vowel + "[aeiou]*";
        private const string Mgr0 = "^(" + ConsonantSequence + ")?" + VowelSequence + ConsonantSequence;
        private const string Meq1 = "^(" + ConsonantSequence + ")?" + VowelSequence + ConsonantSequence + "(" + VowelSequence + ")?$";
        private const string Mgr1 = "^(" + ConsonantSequence + ")?" + VowelSequence + ConsonantSequence + VowelSequence + ConsonantSequence;
        private const string StemVowel = "^(" + ConsonantSequence + ")?" + Vowel;

        private static readonly Regex RX_mgr0 = new Regex(Mgr0);
        private static readonly Regex RX_mgr1 = new Regex(Mgr1);
        private static readonly Regex RX_meq1 = new Regex(Meq1);
        private static readonly Regex RX_s_v = new Regex(StemVowel);

        private static readonly Regex RX1_1a = new Regex("^(.+?)(ss|i)es$");
        private static readonly Regex RX2_1a = new Regex("^(.+?)([^s])s$");
        private static readonly Regex RX1_1b = new Regex("^(.+?)eed$");
        private static readonly Regex RX2_1b = new Regex("^(.+?)(ed|ing)$");
        private static readonly Regex RX1_1b_2 = new Regex(".$");
        private static readonly Regex RX2_1b_2 = new Regex("(at|bl|iz)$");
        private static readonly Regex RX3_1b_2 = new Regex("([^aeiouylsz])\\1$");
        private static readonly Regex RX4_1b_2 = new Regex("^" + ConsonantSequence + Vowel + "[^aeiouwxy]$");

        private static readonly Regex RX1_1c = new Regex("^(.+?[^aeiou])y$");

        private static readonly Regex RX1_2 = new Regex("^(.+?)(ational|tional|enci|anci|izer|bli|alli|entli|eli|ousli|ization|ation|ator|alism|iveness|fulness|ousness|aliti|iviti|biliti|logi)$");

        private static readonly Regex RX1_3 = new Regex("^(.+?)(icate|ative|alize|iciti|ical|ful|ness)$");

        private static readonly Regex RX1_4 = new Regex("^(.+?)(al|ance|ence|er|ic|able|ible|ant|ement|ment|ent|ou|ism|ate|iti|ous|ive|ize)$");

        private static readonly Regex RX2_4 = new Regex("^(.+?)(s|t)(ion)$");

        private static readonly Regex RX1_5 = new Regex("^(.+?)e$");
        private static readonly Regex RX1_5_1 = new Regex("ll$");
        private static readonly Regex RX3_5 = new Regex("^" + ConsonantSequence + Vowel + "[^aeiouwxy]$");
    }
}