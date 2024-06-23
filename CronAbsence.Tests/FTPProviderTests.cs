using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

// Define the FTP options
public class FtpOptions
{
    public string Host { get; set; }
    public string Port { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
}

// Define the interface for the FTP web request
public interface IFtpWebRequest
{
    Task<Stream> GetResponseStreamAsync();
    string Method { get; set; }
    ICredentials Credentials { get; set; }
}

// Implement the FTP web request wrapper
public class FtpWebRequestWrapper : IFtpWebRequest
{
    private readonly FtpWebRequest _ftpWebRequest;

    public FtpWebRequestWrapper(FtpWebRequest ftpWebRequest)
    {
        _ftpWebRequest = ftpWebRequest;
    }

    public string Method
    {
        get => _ftpWebRequest.Method;
        set => _ftpWebRequest.Method = value;
    }

    public ICredentials Credentials
    {
        get => _ftpWebRequest.Credentials;
        set => _ftpWebRequest.Credentials = value;
    }

    public async Task<Stream> GetResponseStreamAsync()
    {
        using var response = (FtpWebResponse)await _ftpWebRequest.GetResponseAsync();
        return response.GetResponseStream();
    }
}

// Define the interface for creating FTP web requests
public interface IFtpWebRequestFactory
{
    IFtpWebRequest Create(Uri uri);
}

// Implement the FTP web request factory
public class FtpWebRequestFactory : IFtpWebRequestFactory
{
    public IFtpWebRequest Create(Uri uri)
    {
        return new FtpWebRequestWrapper((FtpWebRequest)WebRequest.Create(uri));
    }
}

// Define the FTP provider class
public class FTPProvider
{
    private readonly IOptions<FtpOptions> _ftpOptions;
    private readonly IFtpWebRequestFactory _ftpWebRequestFactory;

    public FTPProvider(IOptions<FtpOptions> ftpOptions, IFtpWebRequestFactory ftpWebRequestFactory)
    {
        _ftpOptions = ftpOptions;
        _ftpWebRequestFactory = ftpWebRequestFactory;
    }

    public async Task DownloadFileAsync(string fileName, string localFilePath)
    {
        var ftpOptions = _ftpOptions.Value;
        var ftpUri = new Uri($"ftp://{ftpOptions.Host}:{ftpOptions.Port}/{fileName}");
        var ftpRequest = _ftpWebRequestFactory.Create(ftpUri);
        ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
        ftpRequest.Credentials = new NetworkCredential(ftpOptions.User, ftpOptions.Password);

        using var responseStream = await ftpRequest.GetResponseStreamAsync();
        using var fileStream = new FileStream(localFilePath, FileMode.Create);
        await responseStream.CopyToAsync(fileStream);
    }

    public string GetLocalDownloadPath(string fileName) => Path.Combine(Path.GetTempPath(), fileName);

    public string GetLocalArchivePath(string fileName) => Path.Combine("/archive", fileName);

    public string GetLocalOriginalPath(string fileName) => Path.Combine("/source", fileName);
}

// Define the test class
[TestFixture]
public class FTPProviderTests
{
    private Mock<IOptions<FtpOptions>> _mockFtpOptions;
    private Mock<IFtpWebRequestFactory> _mockFtpWebRequestFactory;
    private FTPProvider _ftpProvider;
    private Mock<IFtpWebRequest> _mockFtpWebRequest;

    [SetUp]
    public void Setup()
    {
        _mockFtpOptions = new Mock<IOptions<FtpOptions>>();
        _mockFtpWebRequestFactory = new Mock<IFtpWebRequestFactory>();
        _mockFtpWebRequest = new Mock<IFtpWebRequest>();
        _ftpProvider = new FTPProvider(_mockFtpOptions.Object, _mockFtpWebRequestFactory.Object);
    }

    [Test]
    public async Task DownloadFileAsync_ShouldDownloadFile()
    {
        // Arrange
        string fileName = "testFile.txt";
        string localFilePath = Path.Combine(Path.GetTempPath(), fileName);

        _mockFtpOptions.Setup(options => options.Value).Returns(new FtpOptions
        {
            Host = "ftp.example.com",
            Port = "21",
            User = "username",
            Password = "password"
        });

        var responseStream = new MemoryStream();
        var fileContent = new byte[] { 0x41, 0x42, 0x43 }; // Sample file content
        responseStream.Write(fileContent, 0, fileContent.Length);
        responseStream.Position = 0;

        _mockFtpWebRequest.Setup(req => req.GetResponseStreamAsync()).ReturnsAsync(responseStream);

        var ftpUri = new Uri("ftp://ftp.example.com:21/testFile.txt");
        _mockFtpWebRequestFactory.Setup(factory => factory.Create(ftpUri)).Returns(_mockFtpWebRequest.Object);

        // Act
        await _ftpProvider.DownloadFileAsync(fileName, localFilePath);

        // Assert
        _mockFtpWebRequest.VerifySet(req => req.Credentials = It.IsAny<NetworkCredential>(), Times.Once);
        _mockFtpWebRequest.Verify(req => req.GetResponseStreamAsync(), Times.Once);

        // Assert file content
        byte[] downloadedContent = File.ReadAllBytes(localFilePath);
        ClassicAssert.AreEqual(fileContent, downloadedContent);
    }

    [Test]
    public void GetLocalDownloadPath_ShouldReturnCorrectPath()
    {
        // Arrange
        string fileName = "testFile.txt";
        string expectedPath = Path.Combine(Path.GetTempPath(), fileName);

        // Act
        string result = _ftpProvider.GetLocalDownloadPath(fileName);

        // Assert
        ClassicAssert.AreEqual(expectedPath, result);
    }

    [Test]
    public void GetLocalArchivePath_ShouldReturnCorrectPath()
    {
        // Arrange
        string fileName = "testFile.txt";
        string expectedPath = Path.Combine("/archive", fileName);

        // Act
        string result = _ftpProvider.GetLocalArchivePath(fileName);

        // Assert
        ClassicAssert.AreEqual(expectedPath, result);
    }

    [Test]
    public void GetLocalOriginalPath_ShouldReturnCorrectPath()
    {
        // Arrange
        string fileName = "testFile.txt";
        string expectedPath = Path.Combine("/source", fileName);

        // Act
        string result = _ftpProvider.GetLocalOriginalPath(fileName);

        // Assert
        ClassicAssert.AreEqual(expectedPath, result);
    }
}
