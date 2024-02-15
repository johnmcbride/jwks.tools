using System.ComponentModel;
using System.Globalization;

public class AlgToEnumConverter : TypeConverter
{
    //a type converter that will convert a string the user passes in
    //to a application enum. if there is an unknown value then we will
    //default to SHA1
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        //get the passed in value and convert it to string
        var hashTypeValue = (string)value;

        //build a switch and return the correct enum based on the passed in
        //value
        switch ( hashTypeValue.ToLower() )
        {
             case "sha1":
                return Enums.HashTypes.SHA1;
            case "sha256":
                return Enums.HashTypes.SHA256;
             case "md5":
                return Enums.HashTypes.MD5;
            default:
                return Enums.HashTypes.SHA1;
        }
    }
}