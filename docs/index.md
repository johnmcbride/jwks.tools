---
layout: default

carousels:
  - images: 
    - image: /assets/images/s1.png
    - image: /assets/images/s2.png
    - image: /assets/images/s3.png
    - image: /assets/images/s4.png
    - image: /assets/images/s5.png
title: JWK(S) Tools and Generator CLI
---

# Overview
JWKS Tools is a .NET global tool that helps build individual JWK, JWKS file from either a single binary ceritificate or a directory of multiple certificate files. It can also be used to verify a JWKS agains a directory of certificate files to verify if there is a discrepency.

## Installation
```sh
dotnet tool install --global jwkstools
```

## Preview Screenshots
{% include carousel.html height="50" unit="%" duration="1000" number="1" %}

## Commands

There are currently three commands in this tool. asd

- **buildjwk** - This command take a certificate and then builds a JWK file from the certificate. Options for command are listed in the table below.

- **buildjwks** - This command takes a directory of one of more certificates and will build a JWKS file with each cert in the keys elements of the file. Options for command are listed in the table below.

- **verifyJWKS** - This command takes a directory with one or more certificates and a JWKS file. It will compare each certificate with whats in the JWKS and report out matches. You can also choose to create a new JWKS file based on the output. Options for command are listed in the table below.

### buildjwk Command

Below are the arguments and options that can be used with the buildjwk command.

<table>
  <tr>
    <th>Name</th>
    <th>Type</th>
    <th>Position</th>
    <th>Description</th>
    <th>Values</th>
    <th>Default</th>
  </tr>
  <tr>
    <td>n/a</td>
    <td>Command Argument</td>
    <td>0</td>
    <td>name (and full path if needed) of a certificate</td>
    <td>n/a</td><td>n/a</td>
  </tr>
  <tr>
    <td>n/a</td>
    <td>Command Argument</td>
    <td>1</td>
    <td>name (and full path if needed) the output file</td>
    <td>n/a</td>
    <td>n/a</td>
  </tr>
  <tr>
    <td>--display</td>
    <td>Option</td>
    <td>n/a</td>
    <td>Output JWK to the console</td>
    <td>n/a</td>
    <td>n/a</td>
  </tr>
  <tr>
    <td nowrap>--overwrite</td>
    <td>Option</td>
    <td>n/a</td>
    <td>Overwrite JWK output file if it exists</td>
    <td>n/a</td>
    <td>n/a</td>
  </tr>
  <tr>
    <td>--hash</td>
    <td>Option</td>
    <td>n/a</td>
    <td>Define what KID hash algorithm should be used</td>
    <td>sha1, sha256, md5</td>
    <td>sha1</td>
  </tr>
  <tr>
    <td>--help</td>
    <td>Option</td>
    <td>n/a</td>
    <td>Get help for the command</td>
    <td></td>
    <td></td>
  </tr>
</table>

### Examples
```sh
jwkstools buildjwk c:\cert\testcert.crt c:\jwkfiles\testcert.jwk --hash sha256

jwkstools buildjwk c:\cert\testcert.crt c:\jwkfiles\testcert.jwk
--display --sha1

jwkstools buildjwk c:\cert\testcert.crt c:\jwkfiles\testcert.jwk --hash sha256 --overwrite
```

### buildjwks Command

Below are the arguments and options that can be used with the buildjwks command.

<table>
  <tr>
    <th>Name</th>
    <th>Type</th>
    <th>Position</th>
    <th>Description</th>
    <th>Values</th>
    <th>Default</th>
  </tr>
  <tr>
    <td>n/a</td>
    <td>Command Argument</td>
    <td>0</td>
    <td>Path of a directory that contains one or more certificates</td>
    <td>n/a</td>
    <td>n/a</td>
  </tr>
  <tr>
      <td>n/a</td>
      <td>Command Argument</td>
      <td>1</td>
      <td>name (and full path if needed) the output JWKS file</td>
      <td>n/a</td>
      <td>n/a</td>
  </tr>
  <tr>
      <td>--display</td>
      <td>Option</td>
      <td>n/a</td>
      <td>Output JWK to the console</td>
      <td>n/a</td>
      <td>n/a</td>
  </tr>
  <tr>
      <td nowrap>--overwrite</td>
      <td>Option</td>
      <td>n/a</td>
      <td>Overwrite JWK output file if it exists</td>
      <td>n/a</td>
      <td>n/a</td>
  </tr>
  <tr>
      <td>--hash</td>
      <td>Option</td>
      <td>n/a</td>
      <td>Define what KID hash algorithm should be used</td>
      <td>sha1, sha256, md5</td>
      <td>sha1</td>
  </tr>
  <tr>
      <td>--help</td>
      <td>Option</td>
      <td>n/a</td>
      <td>Get help for the command</td>
      <td></td>
      <td></td>
  </tr>
</table>

### Examples
```sh
jwkstools buildjwks c:\cert\testcert.crt c:\jwkfiles\testcert.jwk --hash sha256

jwkstools buildjwks c:\certs\ c:\jwkfiles\testcert.jwk
--display --sha1

jwkstools buildjwks c:\cert\testcert.crt c:\jwkfiles\testcert.jwk --hash sha256 --overwrite
```

### verifyjwks Command

Below are the arguments and options that can be used with the verifyjwks command.

<table>
  <tr>
    <th>Name</th>
    <th>Type</th>
    <th>Position</th>
    <th>Description</th>
    <th>Values</th>
    <th>Default</th>
  </tr>
  <tr>
      <td>n/a</td>
      <td>Command Argument</td>
      <td>0</td>
      <td>Path of a directory that contains one or more certificates</td>
      <td>n/a</td>
      <td>n/a</td>
  </tr>
  <tr>
      <td>n/a</td>
      <td>Command Argument</td>
      <td>1</td>
      <td>Path and filename of the JWKS file to verify</td>
      <td>n/a</td>
      <td>n/a</td>
  </tr>
  <tr>
      <td nowrap>--createnew</td>
      <td>Option</td>
      <td>n/a</td>
      <td>Create a new jwks file that is date stamped (in the local where the tool was run)</td>
      <td>n/a</td>
      <td>n/a</td>
  </tr>
  <tr>
      <td>--hash</td>
      <td>Option</td>
      <td>n/a</td>
      <td>Define what KID hash algorithm should be used</td>
      <td>sha1, sha256, md5</td>
      <td>sha1</td>
  </tr>
  <tr>
      <td>--help</td>
      <td>Option</td>
      <td>n/a</td>
      <td>Get help for the command</td>
      <td></td>
      <td></td>
  </tr>
</table>

### Examples
```sh
jwkstools verifyjwks c:\certs\ c:\jwkfiles\newfile.jwks

jwkstools verifyjwks c:\certs\ c:\jwkfiles\newfile.jwks --hash sha1

jwkstools verifyjwks c:\certs\ c:\jwkfiles\newfile.jwks --createnew --hash sha256 --overwrite
```

## More Information

- Issues: Please file issues on github so we can followup and track.
- Contact: Please use github for any communication
- Pull request are welcome. If you would like to add something, feel free to create a pull request and I'll review