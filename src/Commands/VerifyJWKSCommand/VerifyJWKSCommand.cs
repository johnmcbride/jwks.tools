using System.ComponentModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;

namespace Commands;

public class VerifyJWKSCommand : Command<VerifyJWKSCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        var jwks = JObject.Parse(File.ReadAllText(settings.JWKSFile));
        
        //build the extension list
        var certFilesExtensions = new[] { ".crt",".der",".pem"};
        //get the certificate location directory
        var certDirInfo = new DirectoryInfo(settings.CertificatePath);

        //prepare if the user wants to create a new jwks file
        //creating the json objects for the keys property
        dynamic jwksJSON = new JObject();
        jwksJSON.keys = new JArray();

        //loop through each cert in the directory
        foreach ( var file in certDirInfo.GetFiles().Where(f => certFilesExtensions.Contains(f.Extension.ToLower())))
        {
            //load each found cert into the x509 cert object
            var x509 = new X509Certificate2(File.ReadAllBytes(file.FullName));
            //load the key
            var key = new X509SecurityKey(x509);
            //convert the a JWK web key from the token namespace
            var jwk = JsonWebKeyConverter.ConvertFromX509SecurityKey(key, true);

            //export the thumbprint to what the user would like
            string fingerprint = null;
            switch (settings.FingerprintHashType)
            {
                case Enums.HashTypes.SHA1:
                    fingerprint = x509.GetCertHashString(HashAlgorithmName.SHA1);
                    break;
                case Enums.HashTypes.SHA256:
                    fingerprint = x509.GetCertHashString(HashAlgorithmName.SHA256);
                    break;
                case Enums.HashTypes.MD5:
                    fingerprint = x509.GetCertHashString(HashAlgorithmName.MD5);
                    break;
                default:
                    fingerprint = x509.GetCertHashString(HashAlgorithmName.SHA1);
                    break;
            }
            //loop through each jwk in the file to see if the cert is listed
            foreach ( var keySet in jwks["keys"])
            {
                //compare the kid and n values from the cert to each jwk entry
                //if a match then display the information
                if ( keySet["kid"].ToString() == fingerprint && keySet["n"].ToString() == jwk.N.Replace("-","+").Replace("_","/"))
                {
                    AnsiConsole.MarkupLine("[green]FOUND JWK for certificate[/]");
                    AnsiConsole.MarkupLine($"[blue]Certificate Name: {file.Name}[/]");
                    AnsiConsole.MarkupLine($"[blue]Certificate Path: {file.FullName}[/]");
                    AnsiConsole.WriteLine();
                    AnsiConsole.MarkupLine("[green]JWKS Entry[/]");

                    var jwkJson = new JsonText(keySet.ToString());
                    AnsiConsole.Write(jwkJson);
                    jwksJSON.keys.Add(keySet);
                }
            }
        }
        if ( settings.CreateNewFile)
        {
            var filename = $"{DateTime.Now.ToString("MM-dd-yyyy-HHmmss")}.jwks";

            //write out json to the file
            File.WriteAllText(filename,jwksJSON.ToString());

            AnsiConsole.MarkupLine("[yellow]Creating new file with the found JWKs[/]");
            AnsiConsole.MarkupLine($"[blue]Certificate Name: {filename}[/]");
            AnsiConsole.WriteLine();

            AnsiConsole.MarkupLine("[green]NEW JWKS entries[/]");
            var newFileJKWSJson = new JsonText(jwksJSON.ToString());
            AnsiConsole.Write(newFileJKWSJson);
        }
        

        return 0;
    }

    public sealed class Settings : CommandSettings
    {
        [Description("Directory that contains existing binary certificate files (.der, .crt, .pem).")]
        [CommandArgument(0, "<certPath>")]
        public string? CertificatePath { get; init; }

        [Description("Path and filename of the JWKS file to verify.")]
        [CommandArgument(1, "<jwksFile>")]
        public string JWKSFile { get; init; }

        [Description("Option to create a new file with updated values")]
        [CommandOption("-c|--createnew")]
        public bool CreateNewFile { get; set; }

        [Description("The hash output type for the fingerprint/thumbprint")]
        [CommandOption("--hash")]
        [DefaultValue(Enums.HashTypes.SHA1)]
        [TypeConverter(typeof(AlgToEnumConverter))]
        public Enums.HashTypes FingerprintHashType { get; set; }
    }
}