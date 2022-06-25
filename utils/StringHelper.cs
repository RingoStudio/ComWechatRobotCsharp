using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RS_WXBOT_COM.utils
{
    internal class StringHelper
    {
        #region REGEX
        public static bool IsNumeric(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            if (Regex.IsMatch(value, @"^[+-]?0[xX][0-9a-fA-F]+$")) return true;
            if (value == ".") return false;
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }
        public static bool IsInt(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }
        public static bool IsUnsign(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            return Regex.IsMatch(value, @"^\d*[.]?\d*$");
        }
        public static bool IsRID(string rid)
        {
            if (string.IsNullOrEmpty(rid)) return false;
            return Regex.IsMatch(rid, @"^[a-zA-Z0-9]{14}$");
        }
        public static bool isTel(string strInput)
        {
            if (string.IsNullOrEmpty(strInput)) return false;
            return Regex.IsMatch(strInput, @"\d{3}-\d{8}|\d{4}-\d{7}");
        }
        public static bool isQCAccount(string account)
        {

            if (string.IsNullOrEmpty(account)) return false;
            return Regex.IsMatch(account, @"^\\+{0,1}[0-9]+$");
        }
        public static bool isQCPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;
            return Regex.IsMatch(password, @"^[0-9A-Za-z_/+!@#:\\{\\}\\|\\%;\\^\\&\\*-]{6,20}$");
        }
        public static bool IsBase64(string cipher)
        {
            if (string.IsNullOrEmpty(cipher)) return false;
            return Regex.IsMatch(cipher, @"^[A-Za-z0-9+/=]+$");
        }
        #endregion
    }
}
