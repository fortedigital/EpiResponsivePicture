using System;
using System.Collections.Generic;
using EPiServer.ServiceLocation;
using Forte.EpiResponsivePicture.Configuration;
using Forte.EpiResponsivePicture.ResizedImage.Property.Compatibility;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Forte.EpiResponsivePicture.Tests;

[TestFixture]
public class FocalPointParserTests
{
    private IOptions<EpiResponsivePicturesOptions> configuration;

    [SetUp]
    public void Setup()
    {
    }
    
    [TestCase("0.00|0.00", 0, 0)]
    [TestCase("0.01|0.01", 0.01, 0.01)]
    [TestCase("0.42|0.69", 0.42, 0.69)]
    [TestCase("1.00|1.00", 1, 1)]
    public void Forte_EpiResponsivePicture_FocalPoint_Is_Parsed_Correctly(string focalPointBackingString, double expectedX, double expectedY)
    {
        #region Arrange

        configuration = Options.Create(new EpiResponsivePicturesOptions());
            
        ServiceLocator.SetServiceProvider(new DummyServiceLocator(new Dictionary<Type, object>
        {
            {typeof(IOptions<EpiResponsivePicturesOptions>), configuration}
        }));

        #endregion

        #region Act

        var focalPointParser = new FocalPointParser(ServiceLocator.Current.GetInstance<IOptions<EpiResponsivePicturesOptions>>().Value);
        var parsedFocalPoint = focalPointParser.Parse(focalPointBackingString);

        #endregion

        #region Assert

        Assert.AreEqual(expectedX, parsedFocalPoint.X, double.Epsilon);
        Assert.AreEqual(expectedY, parsedFocalPoint.Y, double.Epsilon);

        #endregion
    }
    
    
    [TestCase("{\"x\":0.00,\"y\":0.00}", 0, 0)]
    [TestCase("{\"x\":22.125,\"y\":54.3524416135881}", 0.22, 0.54)]
    [TestCase("{\"x\":100.00,\"y\":100.00}", 1, 1)]
    public void ImageResizer_FocalPoint_Is_Parsed_Correctly(string focalPointBackingString, double expectedX, double expectedY)
    {
        #region Arrange

        configuration = Options.Create(new EpiResponsivePicturesOptions {ImageResizerCompatibilityEnabled = true});
            
        ServiceLocator.SetServiceProvider(new DummyServiceLocator(new Dictionary<Type, object>
        {
            {typeof(IOptions<EpiResponsivePicturesOptions>), configuration}
        }));

        #endregion

        #region Act

        var focalPointParser = new FocalPointParser(ServiceLocator.Current.GetInstance<IOptions<EpiResponsivePicturesOptions>>().Value);
        var parsedFocalPoint = focalPointParser.Parse(focalPointBackingString);

        #endregion

        #region Assert

        Assert.AreEqual(expectedX, parsedFocalPoint.X, double.Epsilon);
        Assert.AreEqual(expectedY, parsedFocalPoint.Y, double.Epsilon);

        #endregion
    }

    [TestCase("{\"x\":22.125,\"y\":54.3524416135881}", "Cannot parse: {\"x\":22.125,\"y\":54.3524416135881} using any of following strategies: ForteResponsivePictureFocalPointParsingStrategy")]
    public void Parsing_Unrecognized_FocalPoint_Format_Should_Give_Meaningful_Error_Message(string focalPointBackingString, string expectedErrorMessage)
    {
        #region Arrange

        configuration = Options.Create(new EpiResponsivePicturesOptions());
            
        ServiceLocator.SetServiceProvider(new DummyServiceLocator(new Dictionary<Type, object>
        {
            {typeof(IOptions<EpiResponsivePicturesOptions>), configuration}
        }));

        #endregion

        #region Act

        var focalPointParser = new FocalPointParser(ServiceLocator.Current.GetInstance<IOptions<EpiResponsivePicturesOptions>>().Value);

        var exception = Assert.Catch<InvalidOperationException>(() =>
        {
            var _ = focalPointParser.Parse(focalPointBackingString);
        });
        

        #endregion

        #region Assert

        Assert.AreEqual(expectedErrorMessage, exception?.Message);

        #endregion
    }
}
