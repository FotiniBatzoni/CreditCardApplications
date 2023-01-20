using Xunit;
using Moq;
using System;

namespace CreditCardApplications.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {
        [Fact]
        public void AcceptingHighIncomeApplecations()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //sut = systemUnderTest
            //we supply null instead new FrequentFlyerNumberValidatorService()
            //var sut = new CreditCardApplicationEvaluator(null);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void ReferYoungApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //we supply null instead new FrequentFlyerNumberValidatorService()
            //var sut = new CreditCardApplicationEvaluator(null);

            mockValidator.DefaultValue = DefaultValue.Mock;

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void DeclineLowIncomeApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey).Returns("OK");

            //1 way
            //to setup isValid to true
            //it returns true only when    FrequentFlyerNumber = "x"

            //1. mockValidator.Setup(x => x.IsValid("x")).Returns(true);

            //2 way
            //it returns true when   FrequentFlyerNumber = is any string

            //2. mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            //3 way
            //it returns true when   FrequentFlyerNumber = is a number which starts with "y"

            //3. mockValidator.Setup(x => x.IsValid(It.Is<string>(number => number.StartsWith("y")))).Returns(true);

            //4 way
            //it returns true when   FrequentFlyerNumber = is in Range from "a" to "z"
            //Moq.Range.Inclusive means that "a" is included to the Range
            //if Moq.Range.Exclusive means that "a" is not included to the Range

            //4. mockValidator.Setup(x => x.IsValid(It.IsInRange<string>("a","z", Range.Inclusive))).Returns(true);

            //5 way
            //it returns true when   FrequentFlyerNumber = is "z" || "y" || "x"

            //5. mockValidator.Setup(x => x.IsValid(It.IsIn("z", "y", "x"))).Returns(true);

            //6 way 
            //it returns true when   FrequentFlyerNumber = match Regex

            mockValidator.Setup(x => x.IsValid(It.IsRegex("[a-z]"))).Returns(true);


            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,
                FrequentFlyerNumber = "y"
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>(MockBehavior.Loose);

            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey).Returns("OK");

            //it is neseccery when we use MockBehavior.Strict

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var aplication = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(aplication);

           Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);   
        }

        //[Fact]
        //public void DeclineLowIncomeApplicationsOutDemo()
        //{
        //    Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

        //    bool isValid = true;
        //    mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));

        //    var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

        //    var application = new CreditCardApplication
        //    {
        //        GrossAnnualIncome = 19_999,
        //        Age = 42
        //    };

        //    CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);

        //    Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        //}

        //[Fact]
        //public void ReferWhenLicenceKeyExpired()
        //{
        //    var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

        //    mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

        //    //Mocking LicenceKey Property
        //    //1 way
        //    //mockValidator.Setup(x => x.LicenceKey).Returns("EXPIRED");

        //    //2 way
        //    //when there is a LicenceKey property
        //    //mockValidator.Setup(x => x.LicenceKey).Returns(GetLicenseKeyExpireString);


        //    var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

        //    var application = new CreditCardApplication {  Age = 42 };

        //    CreditCardApplicationDecision decision = sut.Evaluate(application);

        //    Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        //}

        [Fact]
        public void ReferWhenLicenceKeyExpired()
        {
            //3 way
            //when we use interfaces
            //var mockLisenceData = new Mock<ILicenceData>();
            //mockLisenceData.Setup(x => x.LicenceKey).Returns("EXPIRED");

            //var mockServiceInfo = new Mock<IServiceInformation>();
            //mockServiceInfo.Setup(x => x.Licence).Returns(mockLisenceData.Object);

            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation).Returns(mockServiceInfo.Object);

            //4 way
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey).Returns("EXPIRED");

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);


            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        string GetLicenseKeyExpireString()
        {
            //E.g read from vendor-supplied constants file
            return "EXPIRED";
        }

        [Fact]
        public void UseDetailedLookUpForOlderApplication()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //mockValidator.SetupProperty(x => x.ValidationMode);
            //or
            mockValidator.SetupAllProperties(); //it should be called before making any specific property setups

            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 30 };

            sut.Evaluate(application);

            Assert.Equal(ValidationMode.Deatailed, mockValidator.Object.ValidationMode);
        }

        //[Fact]
        //public void ValidateFrequentFlyerNumberForLowIncomeApplications()
        //{
        //    var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

        //    mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey).Returns("OK");

        //    var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

        //    var application = new CreditCardApplication
        //    {
        //        FrequentFlyerNumber = "q"
        //    };


        //    sut.Evaluate(application);

        //    //"Frequent Flyer Number should be validated" is a Custom Error Message
        //    mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), "Frequent Flyer Number should be validated" );
        //}

        [Fact]
        public void ValidateFrequentFlyerNumberForLowIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey).Returns("OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                FrequentFlyerNumber = "q"
            };


            sut.Evaluate(application);

         
            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Once );
        }

        [Fact]
        public void NotValidateFrequentFlyerNumberForHighIncomeApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey).Returns((string)"OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            sut.Evaluate(application);

            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void CheckLicenceKeyForLowIncomeApplication()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey).Returns((string)"OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 99_000 };


            sut.Evaluate(application);

            //Veryfing a property getter was called ( the property has been accessed)
            mockValidator.VerifyGet(x => x.ServiceInformation.Licence.LicenceKey);
        }

        [Fact]
        public void SetDetailedLookUpForOlderApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey).Returns((string)"OK");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 30 };


            sut.Evaluate(application);

            mockValidator.VerifySet(x => x.ValidationMode = It.IsAny<ValidationMode>(), Times.Once);

            //mockValidator.Verify(x => x.IsValid(null), Times.Once);

            //VerifyNoOtherCalls() means that there should be NO other calls in Moq Object 
            //mockValidator.VerifyNoOtherCalls();
        }

        [Fact]
        public void ReferWhenFrequentFlyerValidationError()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey).Returns((string)"OK");

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Throws<Exception>();
            //for custom error messages
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Throws(new Exception("Custom message"));

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }
    }
}