using System.Collections.Concurrent;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;

namespace Commands;

public class BuildJWKCommand : Command<BuildJWKCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        bool outputExists = false;
        //verify that the passed in cert exists
        if ( !File.Exists(settings.Certificate))
        {
            //generate error and exit
            AnsiConsole.Markup($"[red]Certificate not found.\nCertificate File:{settings.Certificate}[/]");
            return -1;
        }
        //verify that the output file and path
        if ( File.Exists(settings.OutputPath))
        {
            outputExists = true;
        }

        //load the cert
        var x509 = new X509Certificate2(File.ReadAllBytes(settings.Certificate));
        //create a key object
        var key = new X509SecurityKey(x509);
        //create a jwk object using the JsonWebKeyConverter (Microsoft.IdentityModel.Tokens)
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
        // var fingerprint256 = x509.GetCertHashString(HashAlgorithmName.SHA256);
        //get the exponent value
        var exponent = jwk.E;
        //modulus parameter
        var modulus = jwk.N.Replace("-","+").Replace("_","/");
        //get keytype
        var kty = jwk.Kty;

        //Build the JWK
        dynamic jwkJson = new JObject();
        //set key type
        jwkJson.kty = kty;
        //set e (exponent)
        jwkJson.e = exponent;
        //set key identifier (kid)
        jwkJson.kid = fingerprint;
        //set modulus (n)
        jwkJson.n = modulus;

        //if display option is set then show the json to the console
        if ( settings.DisplayOnly)
        {
            var aa = new JsonText(jwkJson.ToString());
            AnsiConsole.Write(aa);
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();
        }
        //if the output does NOT exist then write out the json to the file
        if ( !outputExists )
        {
            //write out json to the file
            File.WriteAllText(settings.OutputPath,jwkJson.ToString());
            var outputFile = new FileInfo(settings.OutputPath);

            AnsiConsole.MarkupLine($"[green]JWK Output file created.[/]");
            AnsiConsole.MarkupLine($"[green]Name: {outputFile.Name}[/]");
            AnsiConsole.MarkupLine($"[green]Fullname: {outputFile.FullName}[/]");
        }
        else //output file exists. Check for additional props
        {
            //check the overwrite option. If true then over the file
            if ( settings.OverwriteOutput)
            {
                AnsiConsole.MarkupLine($"[yellow]Overwriting output file[/]");
                //write out json to the file
                var outputFile = new FileInfo(settings.OutputPath);
                File.WriteAllText(settings.OutputPath,jwkJson.ToString());
                AnsiConsole.MarkupLine($"[green]JWK Output file created.[/]");
                AnsiConsole.MarkupLine($"[green]Name: {outputFile.Name}[/]");
                AnsiConsole.MarkupLine($"[green]Fullname: {outputFile.FullName}[/]");
            }
            else
            {
                //generate error and exit
                AnsiConsole.MarkupLine($"[red]Output file exists! If you would like to overwrite this file please use the [blue]--overwrite[/] option.[/]");
            }
        }
        return 0;
    }

    public sealed class Settings : CommandSettings
    {
        [Description("Full path location and name of the certificate file to build JWK for.")]
        [CommandArgument(0, "<certificate>")]
        public string Certificate { get; set; }

        [Description("Full path location and name of the jwk file that will be created.")]
        [CommandArgument(0, "<outputFile>")]
        public string OutputPath { get; set; }

        [Description("Output JWK to the console and don't write a file.")]
        [CommandOption("-d|--display")]
        public bool DisplayOnly { get; set; } = true;

        [Description("Overwrite the output file if it exists.")]
        [CommandOption("-o|--overwrite")]
        public bool OverwriteOutput { get; set; } = true;

        [Description("The hash output type for the fingerprint/thumbprint")]
        [CommandOption("--hash")]
        [DefaultValue(Enums.HashTypes.SHA1)]
        [TypeConverter(typeof(AlgToEnumConverter))]
        public Enums.HashTypes FingerprintHashType { get; set; }
    }
}