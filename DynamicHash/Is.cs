namespace DynamicHash
{
    public static class Is
    {
        public  static bool AnInt(object value)
        {
            int result;
            return int.TryParse(value.ToString(), out result);
        }
    }
}