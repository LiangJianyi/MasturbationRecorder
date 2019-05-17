namespace MasturbationRecorderTest {
    static class CommonTestResource {
        private static string[] GetTextFile(string path) {
            System.Collections.Generic.List<string> textList = new System.Collections.Generic.List<string>();
            using (System.IO.StreamReader reader = System.IO.File.OpenText(path)) {
                for (string s = reader.ReadLine(); s != null; s = reader.ReadLine()) {
                    textList.Add(s);
                }
            }
            return textList.ToArray();
        }

        public static string[] TestTextLine => GetTextFile(@"C:\Users\a124p\Documents\Test Data\masturbation test.txt");
    }
}
