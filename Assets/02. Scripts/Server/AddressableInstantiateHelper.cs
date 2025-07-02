public static class AddressableInstantiateHelper
{
    private static object[] _tempInstantiationData;

    public static void SetNextInstantiationData(object[] data)
    {
        _tempInstantiationData = data;
    }

    public static object[] ConsumeData()
    {
        var data = _tempInstantiationData;
        _tempInstantiationData = null;
        return data;
    }
}