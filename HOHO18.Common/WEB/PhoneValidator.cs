using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HOHO18.Common.Web{

    public class PhoneValidator {

        static readonly IDictionary<string, Regex> countryRegex = new Dictionary<string, Regex> {
               { "Australia", new Regex(@"^\({0,1}0(2|3|7|8)\){0,1}(\ |-){0,1}[0-9]{4}(\ |-){0,1}[0-9]{4}$")},
               { "Brazil", new Regex(@"^([0-9]{2})?((\([0-9]{2})\)|[0-9]{2})?([0-9]{3}|[0-9]{4})(\-)?[0-9]{4}$")},
               { "Canada", new Regex(@"^((\d[-. ]?)?((\(\d{3}\))|\d{3}))?[-. ]?\d{3}[-. ]?\d{4}$")},
               { "India", new Regex(@"^0{0,1}[1-9]{1}[0-9]{2}[\s]{0,1}[\-]{0,1}[\s]{0,1}[1-9]{1}[0-9]{6}$")},
               { "Italy", new Regex(@"^([+]39)?((38[{8,9}|0])|(34[{7-9}|0])|(36[6|8|0])|(33[{3-9}|0])|(32[{8,9}]))([\d]{7})$")},
               { "Mexico", new Regex(@"^\(\d{3}\) ?\d{3}( |-)?\d{4}|^\d{3}( |-)?\d{3}( |-)?\d{4}")},
               { "Netherlands", new Regex(@"(^\+[0-9]{2}|^\+[0-9]{2}\(0\)|^\(\+[0-9]{2}\)\(0\)|^00[0-9]{2}|^0)([0-9]{9}$|[0-9\-\s]{10}$)")},
               { "Peru", new Regex(@"^([2-9])(\d{2})(-?|\040?)(\d{4})( ?|\040?)(\d{1,4}?|\040?)$")},
               { "South Africa", new Regex(@"^((?:\+27|27)|0)(=72|82|73|83|74|84)(\d{7})$")},
               { "Spain", new Regex(@"^[0-9]{2,3}-? ?[0-9]{6,7}$")},
               { "Sweden", new Regex(@"^(([+]\d{2}[ ][1-9]\d{0,2}[ ])|([0]\d{1,3}[-]))((\d{2}([ ]\d{2}){2})|(\d{3}([ ]\d{3})*([ ]\d{2})+))$")},
               { "UK", new Regex(@"^((\(?0\d{4}\)?\s?\d{3}\s?\d{3})|(\(?0\d{3}\)?\s?\d{3}\s?\d{4})|(\(?0\d{2}\)?\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?$")},
               { "USA", new Regex(@"^[2-9]\d{2}-\d{3}-\d{4}$")},
        };

        public static bool IsValidNumber(string phoneNumber, string country) {
            if (country != null && countryRegex.ContainsKey(country))
                return countryRegex[country].IsMatch(phoneNumber);
            return false;
        }

        public static IEnumerable<string> Countries {
            get {
                return countryRegex.Keys;
            }
        }
    }
}
