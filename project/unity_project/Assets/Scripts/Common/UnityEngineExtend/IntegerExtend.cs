public static class IntegerExtend
{
    public static int[] ToInt32Array(this ushort[] int16Array)
    {
        return System.Array.ConvertAll<ushort, int>(int16Array, Int16ToInt32);
    }

    public static int Int16ToInt32(ushort ushortValue)
    {
        return ushortValue;
    }
}
