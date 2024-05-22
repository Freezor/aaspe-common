namespace aaspe_common.AasxCsharpLibrary.Extensions
{
    public static class ExtendCollection
    {
        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            return list.Count == 0;
        }
    }
}
