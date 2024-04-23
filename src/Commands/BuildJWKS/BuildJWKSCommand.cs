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

public class BuildJWKSCommand : Command<BuildJWKSCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        bool outputExists = false;
        //check if directory exists
        if ( !Directory.Exists(settings.CertificatePath))
        {
            //create directory
            AnsiConsole.Markup($"[red]Certificate directory not found.\nDirectory:{settings.CertificatePath}[/]");
            return -1;
        }
        //check if directory exists
        var outputFileInfo = new FileInfo(settings.JWKSOutputPath);

        if ( !Directory.Exists(outputFileInfo.DirectoryName))
        {
            //create directory
            var fileInfo = new FileInfo(outputFileInfo.DirectoryName);
            fileInfo.Directory.Create();
            AnsiConsole.Markup($"[yellow]Output Directory not found. Creating directory.\nDirectory:{settings.JWKSOutputPath}[/]");
        }
        //check if the output file exists
        if ( File.Exists(settings.JWKSOutputPath))
        {
            outputExists = true;
        }

        //creating the json objects for the keys property
        dynamic jwksJSON = new JObject();
        jwksJSON.keys = new JArray();

        //check if the directory has any files
        if (Directory.GetFiles(settings.CertificatePath).Count() == 0 )
        {
            AnsiConsole.Markup($"[red]Certificate directory does not contain any files.[/]");
            return -1;
        }
        
        //build the extension list
        var certFilesExtensions = new[] { ".crt",".der",".pem"};
        //get the certificate location directory
        var certDirInfo = new DirectoryInfo(settings.CertificatePath);

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

            var exponent = jwk.E;
            var modulus = jwk.N.Replace("-","+").Replace("_","/");;
            var kty = jwk.Kty;

            dynamic jwkJson = new JObject();
            jwkJson.kty = kty;
            jwkJson.e = exponent;
            jwkJson.kid = fingerprint;
            jwkJson.n = modulus;

            jwksJSON.keys.Add(jwkJson);
        }

        //if display option is set then show the json to the console
        if ( settings.DisplayOnly)
        {
            var jsonView = new JsonText(jwksJSON.ToString());
            AnsiConsole.Write(jsonView);
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();
        }

        //if the output does NOT exist then write out the json to the file
        if ( !outputExists )
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[green]JWKS Output file created.[/]");
            AnsiConsole.MarkupLine($"[green]Name: {outputFileInfo.Name}[/]");
            AnsiConsole.MarkupLine($"[green]Fullname: {outputFileInfo.FullName}[/]");

            //write out json to the file
            File.WriteAllText(settings.JWKSOutputPath,jwksJSON.ToString());
        }
        else //output file exists. Check for additional props
        {
            //check the overwrite option. If true then over the file
            if ( settings.OverwriteOutput)
            {
                AnsiConsole.MarkupLine($"[yellow]Overwriting output file[/]");
                //write out json to the file
                File.WriteAllText(settings.JWKSOutputPath,jwksJSON.ToString());
                AnsiConsole.MarkupLine($"[green]JWKS Output file created.[/]");
                AnsiConsole.MarkupLine($"[green]Name: {outputFileInfo.Name}[/]");
                AnsiConsole.MarkupLine($"[green]Fullname: {outputFileInfo.FullName}[/]");
            }
            else
            {
                //generate error and exit
                AnsiConsole.Markup($"[red]Output file exists! If you would like to overwrite this file please use the [blue]--overwrite[/] option.[/]");
            }
        }

        return 0;
    }

    public sealed class Settings : CommandSettings
    {
        [Description("Directory that contains existing binary certificate files (.der, .crt, .pem).")]
        [CommandArgument(0, "<certPath>")]
        public string? CertificatePath { get; init; }

        [Description("Output path and filename of the JWKS output.")]
        [CommandArgument(1, "<outputjwks>")]
        public string JWKSOutputPath { get; init; }

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