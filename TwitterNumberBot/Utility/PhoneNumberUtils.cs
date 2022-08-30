using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TwitterNumberBot.Utility
{
    internal static class PhoneNumberUtils
    {
        public static readonly List<int> US_AREA_CODES = new List<int>
        {
            201, 203, 204, 205, 206, 207, 208, 209, 210, 212, 213, 214, 215,
            216, 217, 218, 219, 220, 223, 224, 225, 228, 229, 231, 234, 239,
            240, 248, 250, 251, 252, 253, 254, 256, 260, 262, 267, 269, 270,
            272, 274, 276, 279, 281, 301, 302, 303, 304, 305, 306, 307, 308,
            309, 310, 312, 313, 314, 315, 316, 317, 318, 319, 320, 321, 323,
            325, 326, 330, 331, 332, 334, 336, 337, 339, 341, 346, 347, 351,
            352, 360, 361, 364, 380, 385, 386, 401, 402, 403, 404, 405, 406,
            407, 408, 409, 410, 412, 413, 414, 415, 416, 417, 418, 419, 423,
            424, 425, 430, 432, 434, 435, 438, 440, 442, 443, 450, 458, 469,
            470, 475, 478, 479, 484, 501, 502, 503, 504, 505, 506, 507, 508,
            509, 510, 512, 513, 514, 515, 516, 517, 518, 519, 530, 531, 534,
            539, 540, 541, 551, 559, 561, 562, 563, 564, 567, 571, 573, 574,
            575, 580, 585, 586, 601, 603, 604, 605, 606, 607, 608, 609, 610,
            612, 613, 614, 615, 616, 617, 618, 619, 620, 626, 628, 629, 630,
            631, 636, 640, 641, 646, 647, 650, 651, 657, 660, 661, 662, 667,
            669, 678, 679, 680, 681, 682, 686, 701, 702, 703, 704, 705, 706,
            707, 708, 709, 712, 713, 714, 715, 716, 717, 718, 719, 720, 724,
            725, 726, 727, 731, 732, 734, 737, 740, 743, 747, 754, 757, 760,
            762, 763, 765, 769, 770, 772, 773, 774, 775, 778, 779, 780, 781,
            785, 786, 801, 802, 803, 804, 805, 806, 807, 808, 810, 812, 813,
            814, 815, 816, 817, 818, 820, 828, 830, 831, 832, 838, 843, 845,
            847, 848, 850, 854, 856, 857, 858, 859, 860, 862, 863, 864, 865,
            867, 870, 872, 877, 878, 901, 902, 903, 904, 905, 906, 907, 908,
            909, 910, 912, 913, 914, 915, 916, 917, 918, 919, 920, 925, 929,
            930, 931, 934, 936, 937, 938, 940, 941, 947, 949, 951, 952, 954,
            956, 959, 970, 971, 972, 973, 978, 979, 980, 984, 985, 986, 989
        };

        public static readonly List<int> TOLL_FREE_AREA_CODES = new List<int>
        {
            800, 822, 833, 844, 855, 866, 877, 888
        };

        public static readonly List<int> CANADIAN_AREA_CODES = new List<int>
        {
            204, 250, 306, 403, 416, 418, 438, 450, 506, 514, 519, 604, 613,
            647, 705, 709, 778, 780, 807, 867, 877, 902, 905
        };

        public static string ValidateNumber(Match phoneNumberIn, bool includeUS = true, bool includeCA = true, bool includeTF = false)
        {
            // Get digits
            var phoneNumber = string.Empty;
            foreach (var c in phoneNumberIn.Value)
            {
                if (int.TryParse(c.ToString(), out var digit))
                    phoneNumber += digit;
            }

            // Check length
            if (phoneNumber.Length > 11 || string.IsNullOrEmpty(phoneNumber))
                return null;

            // Get Area Code
            var areaCode = phoneNumber.StartsWith("1") ?
                int.Parse(phoneNumber.Substring(1, 3)) :
                int.Parse(phoneNumber.Substring(0, 3));

            // Check area code
            var matchedAreaCode = false;
            if (includeTF)
                matchedAreaCode = TOLL_FREE_AREA_CODES.Any(aC => aC.Equals(areaCode));

            if (includeCA && !matchedAreaCode)
                matchedAreaCode = CANADIAN_AREA_CODES.Any(aC => aC.Equals(areaCode));

            if (includeUS && !matchedAreaCode)
                matchedAreaCode = US_AREA_CODES.Any(aC => aC.Equals(areaCode));

            return matchedAreaCode ? phoneNumber : null;
        }
    }
}