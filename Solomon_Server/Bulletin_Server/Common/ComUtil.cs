namespace Solomon_Server.Common
{
    public class ComUtil
    {
        public static int[] GetStringLengths(string item1, string item2)
        {
            int[] stringLengths = new int[2];
            stringLengths[0] = item1.Trim().Length;
            stringLengths[1] = item2.Trim().Length;
            return stringLengths;
        }

        public static int[] GetStringLengths(string item1, string item2, string item3)
        {
            int[] stringLengths = new int[3];
            stringLengths[0] = item1.Trim().Length;
            stringLengths[1] = item2.Trim().Length;
            stringLengths[2] = item3.Trim().Length;
            return stringLengths;
        }

        public static int[] GetStringLengths(string item1, string item2, string item3, string item4)
        {
            int[] stringLengths = new int[4];
            stringLengths[0] = item1.Trim().Length;
            stringLengths[1] = item2.Trim().Length;
            stringLengths[2] = item3.Trim().Length;
            stringLengths[3] = item4.Trim().Length;
            return stringLengths;
        }

        public static int[] GetStringLengths(string item1, string item2, string item3, string item4, string item5)
        {
            int[] stringLengths = new int[5];
            stringLengths[0] = item1.Trim().Length;
            stringLengths[1] = item2.Trim().Length;
            stringLengths[2] = item3.Trim().Length;
            stringLengths[3] = item4.Trim().Length;
            stringLengths[4] = item5.Trim().Length;
            return stringLengths;
        }

        public static int[] GetStringLengths(string item1, string item2, string item3, string item4, string item5, string item6)
        {
            int[] stringLengths = new int[6];
            stringLengths[0] = item1.Trim().Length;
            stringLengths[1] = item2.Trim().Length;
            stringLengths[2] = item3.Trim().Length;
            stringLengths[3] = item4.Trim().Length;
            stringLengths[4] = item5.Trim().Length;
            stringLengths[5] = item6.Trim().Length;
            return stringLengths;
        }
    }
}
