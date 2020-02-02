using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace Realtor.Web.Framework
{
    public static class Util
    {
        private static Dictionary<int, string> saleType = new Dictionary<int, string>() {
            { 1, "매매"}, { 2, "임대"}
        };

        private static Dictionary<int, string> rentType = new Dictionary<int, string>() {
            { 1, "전세"}, { 2, "월세"}
        };

        private static Dictionary<int, string> buildType = new Dictionary<int, string>() {
            { 1, "원룸"}, { 2, "빌라"}, { 3, "다가구"}, { 4, "아파트"}, { 5, "상가"}
        };

        private static Dictionary<int, string> phoneType = new Dictionary<int, string>() {
            { 1, "VIP"}, { 2, "업자"}, { 3, "부동산"}, { 4, "원룸"}, { 5, "기타"}
        };

        public static Dictionary<int, string> SaleType { get { return saleType; } }
        public static Dictionary<int, string> RentType { get { return rentType; } }
        public static Dictionary<int, string> BuildType { get { return buildType; } }
        public static Dictionary<int, string> PhoneType { get { return phoneType; } }

        public static string GetBuildName(int buildType)
        {
            return BuildType[buildType];
        }

        public static string GetPhoneTypeName(int phoneType)
        {
            try
            {
                return PhoneType[phoneType];
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetSaleName(int saleType, int? rentType)
        {
            if (saleType == 1)
            {
                return SaleType[saleType];
            }
            else
            {
                if (rentType.HasValue)
                {
                    return RentType[rentType.Value];
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        
    }
}