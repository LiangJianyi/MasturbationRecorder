using System;
using System.Numerics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MasturbationRecorderTest")]
namespace MasturbationRecorder {
    enum DateMode {
        DateWithWhiteSpace,
        DateWithSlash
    }

    static class DatetimeParser {
        /// <summary>
        /// 转换月份的简写为对应的整数
        /// </summary>
        /// <param name="month">month shorthand</param>
        /// <returns></returns>
        private static ushort StringToUInt16(string month) {
            switch (month.ToLower()) {
                case "jan":
                    return 1;
                case "feb":
                    return 2;
                case "mar":
                    return 3;
                case "apr":
                    return 4;
                case "may":
                    return 5;
                case "jun":
                    return 6;
                case "jul":
                    return 7;
                case "aug":
                    return 8;
                case "sep":
                    return 9;
                case "oct":
                    return 10;
                case "nov":
                    return 11;
                case "dec":
                    return 12;
                default:
                    if (month == "1" ||
                        month == "2" ||
                        month == "3" ||
                        month == "4" ||
                        month == "5" ||
                        month == "6" ||
                        month == "7" ||
                        month == "8" ||
                        month == "9" ||
                        month == "10" ||
                        month == "11" ||
                        month == "12") {
                        return Convert.ToUInt16(month);
                    }
                    else {
                        throw new ArgumentException($"Month acronym error: {month}");
                    }
            }
        }

        /// <summary>
        /// 根据空格切割文本
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string[] GetToken(string text, char separator) {
            return text.Split(separator);
        }

        /// <summary>
        /// 提取频率
        /// </summary>
        /// <param name="frequencyPart"></param>
        /// <returns></returns>
        private static BigInteger GetFrequency(string frequencyPart) {
            if (frequencyPart[0] == 'x' || frequencyPart[0] == 'X') {
                return Convert.ToUInt64(frequencyPart.Substring(1, frequencyPart.Length - 1));
            }
            else if (char.IsWhiteSpace(frequencyPart[0]) || char.IsControl(frequencyPart[0])) {
                return 1;
            }
            else {
                throw new ArgumentException("Repeatable incrementor formal error.");
            }
        }

        /// <summary>
        /// 把每行表达式转换为 StatistTotalByDateTime
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        internal static StatistTotalByDateTime ParseExpr(string expr, DateMode dateMode = DateMode.DateWithWhiteSpace) {
            string[] tokens = null;
            switch (dateMode) {
                case DateMode.DateWithWhiteSpace:
                    tokens = GetToken(expr, ' ');
                    break;
                case DateMode.DateWithSlash:
                    tokens = GetToken(expr, '/');
                    break;
                default:
                    break;
            }
            if (tokens.Length == 3) {
                ushort monthValue = StringToUInt16(tokens[0]);
                ushort dayValue = Convert.ToUInt16(tokens[1]);
                ushort yearValue = Convert.ToUInt16(tokens[2]);
                return new StatistTotalByDateTime() { DateTime = new DateTime(yearValue, monthValue, dayValue), Total = 1 };
            }
            else if (tokens.Length == 4) {
                ushort monthValue = StringToUInt16(tokens[0]);
                ushort dayValue = Convert.ToUInt16(tokens[1]);
                ushort yearValue = Convert.ToUInt16(tokens[2]);
                BigInteger count = GetFrequency(tokens[3]);
                return new StatistTotalByDateTime() { DateTime = new DateTime(yearValue, monthValue, dayValue), Total = count };
            }
            else {
                throw new ArgumentOutOfRangeException($"Date time format error: {tokens}");
            }
        }

        /// <summary>
        /// 每行表达式单独作为一个集合元素
        /// </summary>
        /// <param name="text"></param>
        /// <returns>返回一个表达式序列</returns>
        internal static IEnumerable<string> SplitByLine(string text) {
            return text.Split('\n');
        }
    }
}
