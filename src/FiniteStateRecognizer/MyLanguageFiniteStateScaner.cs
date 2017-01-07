using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiniteStateRecognizer
{
    public static class MyLanguageFiniteStateScaner
    {       
        private static FiniteStateScaner _instance;
        public static FiniteStateScaner Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = new FiniteStateScaner(new TokenRecognizer[]
                {
                    MyLanguage.IdentifiersRecognizer,
                    MyLanguage.DelimetersRecognizer,                
                    MyLanguage.StringsRecognizer,
                    MyLanguage.CharsRecognizer,
                    MyLanguage.IntegersRecognizer,
                    MyLanguage.FloatsRecognizer,
                    MyLanguage.CommentsRecognizer,
                    MyLanguage.MultiLineCommentsRecognizer                
                }.Union(MyLanguage.OperatorRecognizers));

                return _instance;
            }
        }
    }
}
