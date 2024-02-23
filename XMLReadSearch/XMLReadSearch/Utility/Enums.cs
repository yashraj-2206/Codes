namespace Skillup.XMLReadSearch
{
    /// <summary>
    /// File Exception codes
    /// </summary>
    public enum XmlFileExceptionCode
    {
        FileNotExist = 1000,
        NotXmlExtension,
        InvalidFile,
        EmptyFile,
        NoDevicePresent,
        InvalidDeviceInformation
    }

    /// <summary>
    /// Command line exception codes
    /// </summary>
    public enum CommandeLineExceptionCode
    {
        InvalidCommandLineInput = 2000
    }

    /// <summary>
    /// User choices for operation
    /// </summary>
    public enum UserChoices
    {
        ShowAllDevices = 1,
        SearchForDevice,
        Exit

    }

    /// <summary>
    /// Types of devices
    /// </summary>
    public enum DeviceTypes
    {
        A3,
        A4
    }

}
