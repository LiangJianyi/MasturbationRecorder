using MasturbationRecorder;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace MasturbationRecorderTest {
    static class CommonTestResource {
        public static string EXCEPTED_TEXT =
                "May 27 2018 x20\n" +
                "May 29 2018\n" +
                "May 31 2018\n" +
                "Jun 12 2018\n" +
                "Jun 15 2018\n" +
                "Jun 17 2018\n" +
                "Jun 20 2018\n" +
                "Jun 24 2018\n" +
                "Jun 26 2018\n" +
                "Jul 04 2018\n" +
                "Jul 05 2018\n" +
                "Jul 10 2018 x2\n" +
                "Jul 11 2018\n" +
                "Jul 13 2018\n" +
                "Jul 16 2018\n" +
                "Jul 17 2018\n" +
                "Jul 18 2018\n" +
                "Jul 19 2018\n" +
                "Jul 22 2018 x2\n" +
                "Jul 24 2018\n" +
                "Jul 29 2018\n" +
                "Jul 31 2018\n" +
                "Aug 2 2018\n" +
                "Aug 6 2018\n" +
                "Aug 8 2018\n" +
                "Aug 9 2018\n" +
                "Aug 11 2018\n" +
                "Aug 12 2018\n" +
                "Aug 15 2018\n" +
                "Aug 16 2018\n" +
                "Aug 21 2018 x2\n" +
                "Aug 22 2018\n" +
                "Aug 24 2018\n" +
                "Aug 25 2018\n" +
                "Aug 29 2018 x2\n" +
                "Sep 1 2018\n" +
                "Sep 3 2018\n" +
                "Sep 6 2018\n" +
                "Sep 7 2018\n" +
                "Sep 9 2018\n" +
                "Sep 11 2018\n" +
                "Sep 12 2018\n" +
                "Sep 14 2018 x2\n" +
                "Sep 15 2018\n" +
                "Sep 16 2018\n" +
                "Sep 19 2018\n" +
                "Sep 20 2018\n" +
                "Sep 22 2018\n" +
                "Sep 25 2018\n" +
                "Sep 26 2018\n" +
                "Sep 27 2018 x2\n" +
                "Sep 30 2018\n" +
                "Oct 1 2018\n" +
                "Oct 4 2018\n" +
                "Oct 5 2018\n" +
                "Aug 4 2019\n" +
                "Aug 14 2019\n" +
                "Aug 14 2019 x3\n" +
                "Aug 16 2019\n" +
                "Aug 24 2019 x2\n" +
                "Aug 25 2019\n" +
                "Aug 26 2019\n" +
                "Aug 27 2019\n" +
                "Aug 30 2019\n" +
                "Aug 31 2019 x2\n" +
                "Sep 10 2019\n" +
                "Sep 11 2019\n" +
                "Sep 12 2019\n" +
                "Sep 15 2019\n" +
                "Sep 16 2019\n" +
                "Sep 16 2019";

        public static string[] EXCEPTED_LINE = new string[] {
                "May 27 2018 x20" ,
                "May 29 2018" ,
                "May 31 2018" ,
                "Jun 12 2018" ,
                "Jun 15 2018" ,
                "Jun 17 2018" ,
                "Jun 20 2018" ,
                "Jun 24 2018" ,
                "Jun 26 2018" ,
                "Jul 04 2018" ,
                "Jul 05 2018" ,
                "Jul 10 2018 x2" ,
                "Jul 11 2018" ,
                "Jul 13 2018" ,
                "Jul 16 2018" ,
                "Jul 17 2018" ,
                "Jul 18 2018" ,
                "Jul 19 2018" ,
                "Jul 22 2018 x2" ,
                "Jul 24 2018" ,
                "Jul 29 2018" ,
                "Jul 31 2018" ,
                "Aug 2 2018" ,
                "Aug 6 2018" ,
                "Aug 8 2018" ,
                "Aug 9 2018" ,
                "Aug 11 2018" ,
                "Aug 12 2018" ,
                "Aug 15 2018" ,
                "Aug 16 2018" ,
                "Aug 21 2018 x2" ,
                "Aug 22 2018" ,
                "Aug 24 2018" ,
                "Aug 25 2018" ,
                "Aug 29 2018 x2" ,
                "Sep 1 2018" ,
                "Sep 3 2018" ,
                "Sep 6 2018" ,
                "Sep 7 2018" ,
                "Sep 9 2018" ,
                "Sep 11 2018" ,
                "Sep 12 2018" ,
                "Sep 14 2018 x2" ,
                "Sep 15 2018" ,
                "Sep 16 2018" ,
                "Sep 19 2018" ,
                "Sep 20 2018" ,
                "Sep 22 2018" ,
                "Sep 25 2018" ,
                "Sep 26 2018" ,
                "Sep 27 2018 x2" ,
                "Sep 30 2018" ,
                "Oct 1 2018" ,
                "Oct 4 2018" ,
                "Oct 5 2018" ,
                "Aug 4 2019" ,
                "Aug 14 2019" ,
                "Aug 14 2019 x3" ,
                "Aug 16 2019" ,
                "Aug 24 2019 x2" ,
                "Aug 25 2019" ,
                "Aug 26 2019" ,
                "Aug 27 2019" ,
                "Aug 30 2019" ,
                "Aug 31 2019 x2" ,
                "Sep 10 2019" ,
                "Sep 11 2019" ,
                "Sep 12 2019" ,
                "Sep 15 2019" ,
                "Sep 16 2019" ,
                "Sep 16 2019"
        };

        public static StatistTotalByDateTime[] ExceptedStatistTotalByDateTimes = new StatistTotalByDateTime[] {
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 5 , 27) , Total = 20 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 5 , 29) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 5 , 31) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 6 , 12) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 6 , 15) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 6 , 17) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 6 , 20) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 6 , 24) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 6 , 26) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 4) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 5) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 10) , Total = 2 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 11) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 13) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 16) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 17) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 18) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 19) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 22) , Total = 2 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 24) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 29) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 7 , 31) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 2) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 6) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 8) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 9) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 11) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 12) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 15) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 16) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 21) , Total = 2 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 22) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 24) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 25) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 8 , 29) , Total = 2 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 1) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 3) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 6) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 7) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 9) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 11) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 12) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 14) , Total = 2 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 15) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 16) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 19) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 20) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 22) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 25) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 26) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 27) , Total = 2 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 9 , 30) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 10 , 1) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 10 , 4) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2018 , 10 , 5) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 4) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 14) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 14) , Total = 3 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 16) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 24) , Total = 2 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 25) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 26) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 27) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 30) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 8 , 31) , Total = 2 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 9 , 10) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 9 , 11) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 9 , 12) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 9 , 15) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 9 , 16) , Total = 1 },
                new StatistTotalByDateTime() { DateTime = new DateTime( 2019 , 9 , 16) , Total = 1 }
            };

        /// <summary>
        /// 生成一个 Configuration
        /// </summary>
        public async static Task<Configuration> GenerateConfigurationAsync() =>
            new Configuration(
                username: KeyGenerator.GetUniqueKey(10),
                password: KeyGenerator.GetUniqueKey(256),
                title: KeyGenerator.GetUniqueKey(12),
                theme: Theme.Light,
                avatar: await KnownFolders.PicturesLibrary.GetFileAsync("wangyuyan.png")
            );
    }
}
